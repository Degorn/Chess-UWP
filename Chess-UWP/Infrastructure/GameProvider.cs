using Chess_UWP.Infrastructure.Initializers;
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
        public FigureState CurrentlySelectedFigure
        {
            get => currentlySelectedFigure;
            set
            {
                if (value == null)
                {
                    if (currentlySelectedFigure != null)
                    {
                        currentlySelectedFigure.Selected = false;
                    }
                }
                currentlySelectedFigure = value;
                if (value != null)
                {
                    currentlySelectedFigure.Selected = true;
                }
            }
        }

        private readonly IFiguresInitializer figuresInitializer;

        #region Constructors

        public GameProvider(IFiguresInitializer figuresInitializer)
        {
            this.figuresInitializer = figuresInitializer;
            FiguresOnBoard = figuresInitializer.GetFigures().ToList();
            syncContext = SynchronizationContext.Current;

            StartGameTimer();
        }

        public GameProvider(IFiguresInitializer figuresInitializer, Player[] players) : this(figuresInitializer)
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

        private void AddFigure(Figure figure, Point position, Color color)
        {
            FigureState newFigure = figuresInitializer.GetFigure(figure, position, color);
            FiguresOnBoard.Add(newFigure);
            CollectionChanged(this, new CollectionChangedEventHandler
            {
                Operation = ListChagesOperation.Add,
                Item = newFigure
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

        private IEnumerable<Direction> GetFigureDirections(FigureState figure, bool includeCheckmateState = true)
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
                    else if (includeCheckmateState && !CheckIfMoveIsSaveForKing(figure, potentialPosition))
                    {
                        continue;
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

        public IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool includeCheckmateState = true)
        {
            IEnumerable<Direction> directions = GetFigureDirections(figure, includeCheckmateState);
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
            if (CurrentlySelectedFigure != null)
            {
                TryToMoveTo(CurrentlySelectedFigure, position);
                CurrentlySelectedFigure = null;
            }

            FigureState figure = GetCurrentPlayerFigureByPosition(position);
            if (figure != null)
            {
                CurrentlySelectedFigure = figure;
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
            figureStartPosition = figure.Position;
            figure.Position = position;
            figure.Figure.Step();

            // Pawn promotion.
            if (CheckPawnPromotion(figure))
            {
                StartPawnPromotion(this, new PawnPromotionEventArgs
                {
                    Types = GetPawnPromotionTypes()
                });
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
                    Winner = EnemyPlayer,
                    GameLength = TimeSpan.FromSeconds(gameLengthInSeconds).ToString(@"hh\:mm\:ss")
                });
            }

            LogMove(this, new MoveLogEventArgs
            {
                Figure = currentlySelectedFigure.Figure,
                Color = currentlySelectedFigure.Color,
                StartPosition = AdaptPositionToBoard(figureStartPosition),
                EndPosition = AdaptPositionToBoard(currentlySelectedFigure.Position)
            });

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
            if (CurrentlySelectedFigure != null)
            {
                CurrentlySelectedFigure = null;
            }

            ResetMoveTimer();
        }

        #endregion

        #region Castling

        // First pair - King and his positon for castling; second pair - Rook for castling and it's positions to move.
        private Dictionary<Tuple<FigureState, Point>, Tuple<FigureState, Point>> castlingConditions = new Dictionary<Tuple<FigureState, Point>, Tuple<FigureState, Point>>();

        private bool CheckIfTryingToCastle(Point position)
        {
            if (!(CurrentlySelectedFigure.Figure is King))
            {
                return false;
            }

            return castlingConditions.ContainsKey(new Tuple<FigureState, Point>(CurrentlySelectedFigure, position));
        }

        private FigureState GetRookForCastling(bool isKingSide, int yPos)
        {
            int xPos = isKingSide ? Board.BOARD_WIDTH - 1 : 0;
            return FiguresOnBoard.FirstOrDefault(f => f.Position == new Point(xPos, yPos) && f.Figure.GetType() == typeof(Rook) && f.Figure.IsNeverMoved);
        }

        private Tuple<FigureState, Point> GetCastlingRook(Point position)
        {
            return castlingConditions[new Tuple<FigureState, Point>(CurrentlySelectedFigure, position)];
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

        private IEnumerable<Direction> GetAllFiguresDirectionsByColor(Color color)
        {
            foreach (FigureState figure in FiguresOnBoard.Where(f => f.Color == color && f.Figure.GetType() != typeof(King)))
            {
                foreach (Direction direction in GetFigureDirections(figure, false))
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
            IEnumerable<Point> possiblePositionsOfKing = GetPossibleFigurePositions(kingOfTheCurrentPlayer, false);
            IEnumerable<Point> enemyDirectionsPositions = GetAllFiguresPositionsByColor(EnemyPlayer.Color);
            foreach (Point kingPosition in possiblePositionsOfKing)
            {
                if (!enemyDirectionsPositions.Contains(kingPosition) && CheckIfMoveIsSaveForKing(kingOfTheCurrentPlayer, kingPosition))
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

        private bool CheckIfMoveIsSaveForKing(FigureState figureToMove, Point potentialFigurePosition)
        {
            FigureState king = GetKing(CurrentPlayer.Color);
            if (king == null)
            {
                return true;
            }
            Point kingPosition = figureToMove.Figure is King ? potentialFigurePosition : king.Position;

            int[,] enemyFiguresDirections = new int[8, 8];
            IEnumerable<FigureState> enemyFigures = FiguresOnBoard.Where(f => f.Color == EnemyPlayer.Color);
            if (enemyFigures == null)
            {
                return true;
            }
            foreach (FigureState figure in enemyFigures)
            {
                IEnumerable<Direction> figureDirections = figure.Figure.PossiblePositionsToMove();
                foreach (Direction direction in figureDirections)
                {
                    if (figure.Position == potentialFigurePosition)
                    {
                        break;
                    }

                    foreach (Point vector in direction.Positions)
                    {
                        Point potentialPosition = figure.Position.Add(vector);
                        FigureState figureOnPosition = GetFigureByPosition(potentialPosition);
                        if (figureToMove.Position == potentialPosition)
                        {
                            figureOnPosition = null;
                        }

                        if (figure.Figure is Pawn && potentialPosition.Subtract(figure.Position).X == 0) // Skip pawn forward moving because they can't beat.
                        {
                            break;
                        }
                        if (potentialPosition.CheckIfOutsideTheBoard())
                        {
                            break;
                        }

                        enemyFiguresDirections[(int)potentialPosition.Y, (int)potentialPosition.X]++;
                        if (potentialPosition == potentialFigurePosition)
                        {
                            break;
                        }

                        if (figureOnPosition != null && potentialPosition == figureOnPosition.Position)
                        {
                            break;
                        }

                        if (potentialPosition == kingPosition)
                        {
                            return false;
                        }
                    }
                }
            }

            return enemyFiguresDirections[(int)kingPosition.Y, (int)kingPosition.X] == 0;
        }

        #endregion

        #region Pawn promotion

        public event PawnPromotionDelegate StartPawnPromotion;

        private bool CheckPawnPromotion(FigureState pawn)
        {
            if (pawn.Figure.GetType() != typeof(Pawn))
            {
                return false;
            }

            return pawn.Color == Color.Black && pawn.Position.Y == BOARD_HEIGHT - 1 ||
                   pawn.Color == Color.White && pawn.Position.Y == 0;
        }

        private IEnumerable<string> GetPawnPromotionTypes()
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
            
            AddFigure(figure, pawn.Position, pawn.Color);
            RemoveFigure(pawn);
        }

        public void PromotePawn(string type)
        {
            Type selectedType = Type.GetType($"Chess_UWP.Models.Figures.{type}");
            PromotePawnWith(CurrentlySelectedFigure, selectedType);
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
                CurrentlySelectedFigure.Figure is Pawn &&
                potentialPosition == enPassantPosition;
        }

        #endregion

        #region Timer

        private Timer gameTimer;
        private int gameLengthInSeconds;

        private SynchronizationContext syncContext;
        private Timer moveTimer;
        public int SecondsOnMove { get; private set; }
        private int secondsLeft;

        public event TimerTickDelegate TimerTick;

        private void MoveTimerTick(object state)
        {
            syncContext.Post(a =>
            {
                TimerTick?.Invoke(this, new TimerTickEventArgs { SecondsLeft = secondsLeft });
                secondsLeft--;
                if (secondsLeft < 0)
                {
                    MoveFinalizer();
                }
            }, null);
        }

        public void SetMoveTimer(int secondsOnMove)
        {
            SecondsOnMove = secondsOnMove;
        }

        public void StartMoveTimer()
        {
            if (SecondsOnMove == 0)
            {
                return;
            }

            secondsLeft = SecondsOnMove;
            moveTimer = new Timer(MoveTimerTick, null, 1000, 1000);
        }

        private void StopMoveTimer()
        {
            moveTimer?.Dispose();
        }

        private void ResetMoveTimer()
        {
            StopMoveTimer();
            StartMoveTimer();
        }

        private void StartGameTimer()
        {
            gameLengthInSeconds = 0;
            gameTimer = new Timer(GameTimerTick, null, 1000, 1000);
        }

        private void GameTimerTick(object state)
        {
            gameLengthInSeconds++;
        }

        private void StopGameTimer()
        {
            gameTimer?.Dispose();
        }

        #endregion

        #region Move logger

        public event MoveLogDelegate LogMove;

        private Point figureStartPosition;

        private Point AdaptPositionToBoard(Point position)
        {
            return new Point(position.X, Board.BOARD_HEIGHT - position.Y);
        }

        #endregion
    }
}