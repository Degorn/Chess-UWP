﻿using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure
{
    public class GameProvider : IGameProvider
    {
        private ICollection<FigureState> FiguresOnBoard { get; set; }
        public event CollectionChanged CollectionChanged;

        private int playerId = 0;
        private readonly Player[] players;
        private Player CurrentPlayer => players?[playerId];
        private Player EnemyPlayer => players?.FirstOrDefault(p => p != CurrentPlayer);

        private FigureState currentlySelectedFigure;

        private IFiguresImagesInitializer ImagesInitializer { get; }

        #region Constructors

        public GameProvider(IFiguresInitializer figuresInitializer, IFiguresImagesInitializer imagesInitializer)
        {
            ImagesInitializer = imagesInitializer;
            FiguresOnBoard = figuresInitializer.GetFigures(imagesInitializer).ToList();
            syncContext = SynchronizationContext.Current;
        }

        public GameProvider(IFiguresInitializer figuresInitializer, IFiguresImagesInitializer imagesInitializer, Player[] players) : this(figuresInitializer, imagesInitializer)
        {
            this.players = players;
        }

        #endregion

        #region Helpers

        private FigureState GetFigureByPosition(Point position)
        {
            return FiguresOnBoard.FirstOrDefault(f => f.Position.Equals(position));
        }

        private FigureState GetCurrentPlayerFigureByPosition(Point position)
        {
            return FiguresOnBoard.FirstOrDefault(f => f.Position.Equals(position) && f.Color == CurrentPlayer.Color);
        }

        private FigureState GetKing(Color color)
        {
            return FiguresOnBoard.FirstOrDefault(f => f.Figure.GetType() == typeof(King) && f.Color == color);
        }

        private void AddFigure(FigureState figure)
        {
            FiguresOnBoard.Add(figure);
            CollectionChanged(this, new CollectionChangedEventHandler
            {
                Operation = ListChagesOperation.Add,
                Item = figure
            });
        }

        private void RemoveFigure(FigureState figure)
        {
            FiguresOnBoard.Remove(figure);
            CollectionChanged(this, new CollectionChangedEventHandler
            {
                Operation = ListChagesOperation.Remove,
                Item = figure
            });
        }

        #endregion

        #region Main functionality

        public IEnumerable<FigureState> GetFigures()
        {
            return FiguresOnBoard;
        }

        private IEnumerable<Direction> GetFigureDirections(FigureState figure, bool isPotentialCalculation = false)
        {
            IEnumerable<Direction> figureDirections = figure.Figure.PossiblePositionsToMove();

            foreach (Direction direction in figureDirections)
            {
                Direction newDirection = new Direction
                {
                    StartPosition = figure.Position
                };

                foreach (Point vector in direction.Positions)
                {
                    Point potentialPosition = figure.Position.Add(vector);
                    FigureState figureOnPosition = GetFigureByPosition(potentialPosition);

                    if (potentialPosition.CheckIfOutsideTheBoard() ||
                        figureOnPosition?.Color == figure.Color)
                    {
                        break;
                    }
                    else if (!isPotentialCalculation && CheckCheckStateAfterMove(figure, potentialPosition))
                    {
                        break;
                    }
                    else if (CheckIfFigureCanMoveTo(figure, potentialPosition))
                    {
                        newDirection.Positions.Add(potentialPosition);
                    }
                    else if (CheckIfFigureCanBeatTo(figure, potentialPosition))
                    {
                        newDirection.Positions.Add(potentialPosition);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }

                yield return newDirection;
            }
        }

        public IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool isPotentialCalculation = false)
        {
            IEnumerable<Direction> directions = GetFigureDirections(figure, isPotentialCalculation);
            foreach (Direction direction in directions)
            {
                foreach (Point position in direction.Positions)
                {
                    yield return position;
                }
            }
            
            // Add castling position;
            IEnumerable<Point> castlingPositions = GetCastlingPositions(figure);
            foreach (Point castlingPosition in castlingPositions)
            {
                yield return castlingPosition;
            }
        }

        public void DoActionByPositions(Point position)
        {
            if (currentlySelectedFigure != null)
            {
                TryToMoveTo(currentlySelectedFigure, position);
            }

            FigureState figure = GetCurrentPlayerFigureByPosition(position);
            if (figure != null)
            {
                currentlySelectedFigure = figure;
                currentlySelectedFigure.Selected = true;
            }
        }

        public void ResetFigures(IEnumerable<FigureState> figures)
        {
            FiguresOnBoard = figures.ToList();
        }

        private void TryToMoveTo(FigureState figure, Point position)
        {
            if (!GetPossibleFigurePositions(figure).Contains(position))
            {
                return;
            }

            // Castling.
            if (CheckIfTryingToCastle(position))
            {
                Tuple<FigureState, Point> rookAndPosition = GetCastlingRook(position);
                rookAndPosition.Item1.Position = rookAndPosition.Item2;
            }

            // Beating.
            FigureState figureOnPosition = GetFigureByPosition(position);
            if (figureOnPosition != null)
            {
                RemoveFigure(figureOnPosition);
            }

            // En passant.
            if (position == enPassantPosition)
            {
                RemoveFigure(enPassantPawn);
            }
            enPassantPawn = null;
            if (figure.Figure is Pawn &&
                Math.Abs(figure.Position.Y - position.Y) == 2)
            {
                enPassantPawn = figure;
            }
            
            // Moving.
            figure.Position = position;
            figure.Figure.Step();

            // Pawn promotion.
            if (CheckPawnPromotion(figure))
            {
                StartPawnPromotion(this, EventArgs.Empty);
                return;
            }

            // Player switching and checking checkmate state.
            MoveFinalizer();
        }

        private void MoveFinalizer()
        {
            SwitchPlayer();

            CheckmateState state = GetCheckmateState();
            if (state == CheckmateState.Checkmate)
            {
                GameOver(this, new GameOverEventArgs
                {
                    Winner = EnemyPlayer
                });
            }

            ResetState();
        }

        private bool CheckIfFigureCanMoveTo(FigureState figure, Point potentialPosition)
        {
            FigureState figureOnPotentialPosition = GetFigureByPosition(potentialPosition);

            if (figureOnPotentialPosition == null)
            {
                if (figure.Figure is Pawn)
                {
                    if (potentialPosition.Subtract(figure.Position).X == 0) // If trying to move vertically.
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckIfFigureCanBeatTo(FigureState figure, Point potentialPosition)
        {
            FigureState figureOnPotentialPosition = GetFigureByPosition(potentialPosition);

            if (figure.Figure is Pawn)
            {
                if (potentialPosition.Subtract(figure.Position).X != 0) // If trying to move diagonally.
                {
                    return (figureOnPotentialPosition != null &&
                           figureOnPotentialPosition.Color != figure.Color) ||
                           CheckEnPassant(potentialPosition);
                }
            }
            else
            {
                return figureOnPotentialPosition != null &&
                       figureOnPotentialPosition.Color != figure.Color;
            }

            return false;
        }

        private void SwitchPlayer()
        {
            if (players == null || players.Length == 0)
            {
                return;
            }

            playerId++;
            if (playerId >= players.Length)
            {
                playerId = 0;
            }
        }

        private void ResetState()
        {
            if (currentlySelectedFigure != null)
            {
                currentlySelectedFigure.Selected = false;
                currentlySelectedFigure = null;
            }

            ResetTimer();
        }

        #endregion

        #region Castling

        // First pair - King and his positon for castling; second pair - Rook for castling and it's positions to move.
        private Dictionary<Tuple<FigureState, Point>, Tuple<FigureState, Point>> castlingConditions = new Dictionary<Tuple<FigureState, Point>, Tuple<FigureState, Point>>();

        private bool CheckIfTryingToCastle(Point position)
        {
            if (!(currentlySelectedFigure.Figure is King))
            {
                return false;
            }

            return castlingConditions.ContainsKey(new Tuple<FigureState, Point>(currentlySelectedFigure, position));
        }

        private FigureState GetRookForCastling(bool isKingSide, int yPos)
        {
            int xPos = isKingSide ? Board.BOARD_WIDTH - 1 : 0;
            return FiguresOnBoard.FirstOrDefault(f => f.Position == new Point(xPos, yPos) && f.Figure.GetType() == typeof(Rook) && f.Figure.IsNeverMoved);
        }

        private Tuple<FigureState, Point> GetCastlingRook(Point position)
        {
            return castlingConditions[new Tuple<FigureState, Point>(currentlySelectedFigure, position)];
        }

        private IEnumerable<Point> GetCastlingPositions(FigureState kingOnBoard)
        {
            if (!(kingOnBoard.Figure is King king) ||
                !king.IsNeverMoved)
            {
                yield break;
            }

            castlingConditions = new Dictionary<Tuple<FigureState, Point>, Tuple<FigureState, Point>>();
            int kingXPos = (int)kingOnBoard.Position.X;
            int kingYPos = (int)kingOnBoard.Position.Y;
            List<FigureState> rooks = new List<FigureState>
            {
                GetRookForCastling(true, kingYPos),
                GetRookForCastling(false, kingYPos)
            };
            foreach (FigureState rook in rooks)
            {
                if (rook == null || kingOnBoard.Color != rook.Color)
                {
                    continue;
                }

                bool isPathClear = true;
                int rookXPos = (int)rook.Position.X;
                int fromX, toX;
                Point additionalVector;
                Point additionalRooksPotentialPosition;

                if (rookXPos < kingXPos)
                {
                    fromX = rookXPos;
                    toX = kingXPos;
                    additionalVector = new Point(-2, 0);
                    additionalRooksPotentialPosition = kingOnBoard.Position.Add(additionalVector).Add(new Point(1, 0));
                }
                else
                {
                    fromX = kingXPos;
                    toX = rookXPos;
                    additionalVector = new Point(2, 0);
                    additionalRooksPotentialPosition = kingOnBoard.Position.Add(additionalVector).Add(new Point(-1, 0));
                }

                for (int x = fromX + 1; x < toX; x++)
                {
                    if (GetFigureByPosition(new Point(x, kingYPos)) != null)
                    {
                        isPathClear = false;
                        break;
                    }
                }

                if (isPathClear)
                {
                    Point kingsPotentialPosition = kingOnBoard.Position.Add(additionalVector);
                    castlingConditions.Add(new Tuple<FigureState, Point>(kingOnBoard, kingsPotentialPosition), new Tuple<FigureState, Point>(rook, additionalRooksPotentialPosition));
                    yield return kingsPotentialPosition;
                }
            }
        }

        #endregion

        #region Checkmate state

        public event GameOverDelegate GameOver;

        public enum CheckmateState
        {
            None, Check, Checkmate
        }

        private IEnumerable<Direction> GetAllFiguresDirectionsByColor(Color color)
        {
            foreach (FigureState figure in FiguresOnBoard.Where(f => f.Color == color && f.Figure.GetType() != typeof(King)))
            {
                foreach (Direction direction in GetFigureDirections(figure, true))
                {
                    yield return direction;
                }
            }
        }

        private IEnumerable<Point> GetAllFiguresPositionsByColor(Color color)
        {
            foreach (Direction direction in GetAllFiguresDirectionsByColor(color))
            {
                foreach (Point position in direction.Positions)
                {
                    yield return position;
                }
            }
        }

        public CheckmateState GetCheckmateState()
        {
            if (CurrentPlayer == null)
            {
                return CheckmateState.None;
            }
            FigureState kingOfCurrentPlayer = GetKing(CurrentPlayer.Color);
            if (kingOfCurrentPlayer == null)
            {
                return CheckmateState.None;
            }

            if (ChechCheckmateState())
            {
                return CheckmateState.Checkmate;
            }
            else if (ChechCheckState())
            {
                return CheckmateState.Check;
            }

            return CheckmateState.None;
        }

        private bool ChechCheckState()
        {
            FigureState kingOfCurrentPlayer = GetKing(CurrentPlayer.Color);
            if (kingOfCurrentPlayer == null)
            {
                return false;
            }

            IEnumerable<Direction> enemyDirections = GetAllFiguresDirectionsByColor(EnemyPlayer.Color);
            foreach (Direction enemyDirection in enemyDirections)
            {
                if (enemyDirection.Positions.Contains(kingOfCurrentPlayer.Position))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ChechCheckmateState()
        {
            FigureState kingOfTheCurrentPlayer = GetKing(CurrentPlayer.Color);

            List<Direction> threateningDirections = new List<Direction>();
            IEnumerable<Direction> enemyDirections = GetAllFiguresDirectionsByColor(EnemyPlayer.Color);
            foreach (Direction direction in enemyDirections)
            {
                if (direction.Positions.Contains(kingOfTheCurrentPlayer.Position))
                {
                    threateningDirections.Add(direction);
                }
            }

            // Сheck for threats to the king.
            if (threateningDirections.Count == 0)
            {
                return false;
            }

            // Check if the king can move in a safe place.
            IEnumerable<Point> possiblePositionsOfKing = GetPossibleFigurePositions(kingOfTheCurrentPlayer, true);
            IEnumerable<Point> enemyDirectionsPositions = GetAllFiguresPositionsByColor(EnemyPlayer.Color);
            foreach (Point kingPosition in possiblePositionsOfKing)
            {
                if (!enemyDirectionsPositions.Contains(kingPosition) && !CheckCheckStateAfterMove(kingOfTheCurrentPlayer, kingPosition))
                {
                    return false;
                }
            }

            // Check if any of figures can block the king.
            IEnumerable<Point> currentPlayerFiguresPositions = GetAllFiguresPositionsByColor(CurrentPlayer.Color);
            foreach (Point potentialCurrentPlayerFigurePosition in currentPlayerFiguresPositions)
            {
                if (CheckIfPointBlocksDirections(threateningDirections, kingOfTheCurrentPlayer.Position, potentialCurrentPlayerFigurePosition))
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckIfPointBlocksDirections(IEnumerable<Direction> directions, Point target, Point pointToBlockDirection)
        {
            bool isPointBlocksDirections = false;
            foreach (Direction direction in directions)
            {
                if (direction.Positions.Contains(target) && CheckIfPointBlocksDirection(direction, target, pointToBlockDirection))
                {
                    isPointBlocksDirections = true;
                }
            }

            return isPointBlocksDirections;
        }

        private bool CheckIfPointBlocksDirection(Direction direction, Point target, Point pointToBlockDirection)
        {
            List<Point> remainingPositions = new List<Point>();
            foreach (Point directionPoint in direction.Positions)
            {
                if (pointToBlockDirection == direction.StartPosition || pointToBlockDirection == directionPoint)
                {
                    break;
                }
                remainingPositions.Add(directionPoint);
            }

            return !remainingPositions.Contains(target);
        }

        /// <summary>
        /// Block a figure’s ability to move if this leads to a check.
        /// </summary>
        /// <param name="figure">Figure to move.</param>
        /// <param name="potentialPosition">Potential position of a figure.</param>
        /// <returns>True if movement leads to a check.</returns>
        private bool CheckCheckStateAfterMove(FigureState figure, Point potentialPosition)
        {
            FigureState kingOfCurrentPlayer = GetKing(CurrentPlayer.Color);
            if (kingOfCurrentPlayer == null)
            {
                return false;
            }

            FigureState figureOnPotentialPosition = GetFigureByPosition(potentialPosition);
            if (figureOnPotentialPosition != null)
            {
                FiguresOnBoard.Remove(figureOnPotentialPosition);
            }

            Point currentPosition = figure.Position;
            figure.Position = potentialPosition;
            bool result = ChechCheckState();
            figure.Position = currentPosition;

            if (figureOnPotentialPosition != null)
            {
                FiguresOnBoard.Add(figureOnPotentialPosition);
            }

            return result;
        }

        #endregion

        #region Pawn promotion

        public event EventHandler StartPawnPromotion;

        private bool CheckPawnPromotion(FigureState pawn)
        {
            if (pawn.Figure.GetType() != typeof(Pawn))
            {
                return false;
            }

            return pawn.Color == Color.Black && pawn.Position.Y == BOARD_HEIGHT - 1 ||
                   pawn.Color == Color.White && pawn.Position.Y == 0;
        }

        public IEnumerable<string> GetPawnPromotionTypes()
        {
            return new string[]
            {
                "Rook", "Knight", "Bishop", "Queen"
            };
        }

        private void PromotePawnWith(FigureState pawn, Type figureType)
        {
            if (pawn.Figure.GetType() != typeof(Pawn))
            {
                return;
            }

            Figure figure;
            Type type = figureType;
            if (type == typeof(Rook)) figure = new Rook();
            else if (type == typeof(Knight)) figure = new Knight();
            else if (type == typeof(Bishop)) figure = new Bishop();
            else if (type == typeof(Queen)) figure = new Queen();
            else return;

            FigureState newFigure = new FigureState(figure, pawn.Position, pawn.Color);
            newFigure.Figure.Image = ImagesInitializer.GetImage(type, newFigure.Color);
            AddFigure(newFigure);
            RemoveFigure(pawn);
        }

        public void PromotePawn(string type)
        {
            Type selectedType = Type.GetType($"Chess_UWP.Models.Figures.{type}");
            PromotePawnWith(currentlySelectedFigure, selectedType);
            MoveFinalizer();
        }

        #endregion

        #region En passant

        private FigureState enPassantPawn;
        private Point enPassantPosition
        {
            get
            {
                if (enPassantPawn == null) return new Point();
                double yPos;
                yPos = enPassantPawn.Color == Color.Black ? enPassantPawn.Position.Y - 1 : enPassantPawn.Position.Y + 1;
                return new Point(enPassantPawn.Position.X, yPos);
            }
        }

        private bool CheckEnPassant(Point potentialPosition)
        {
            return enPassantPawn != null &&
                currentlySelectedFigure.Figure is Pawn &&
                potentialPosition == enPassantPosition;
        }

        #endregion

        #region Timer

        private SynchronizationContext syncContext;
        private Timer timer;
        public int SecondsOnMove { get; private set; }
        private int secondsLeft;

        public event TimerTickDelegate TimerTick;

        private void OnTick(object state)
        {
            secondsLeft--;
            syncContext.Post(a =>
            {
                TimerTick?.Invoke(this, new TimerTickEventArgs { SecondsLeft = secondsLeft });
            }, null);

            if (secondsLeft <= 0)
            {
                MoveFinalizer();
            }
        }

        public void SetTimerOnMove(int secondsOnMove)
        {
            SecondsOnMove = secondsOnMove;
        }

        public void StartTimer()
        {
            if (SecondsOnMove == 0)
            {
                return;
            }

            secondsLeft = SecondsOnMove;
            timer = new Timer(OnTick, null, 1000, 1000);
        }

        private void StopTimer()
        {
            timer?.Dispose();
        }

        private void ResetTimer()
        {
            StopTimer();
            StartTimer();
        }

        #endregion
    }
}