using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Tests
{
    [TestClass]
    public class Tests
    {
        private GameProvider gameProvider;

        public Tests()
        {
            IFiguresInitializer figuresInitializer = new FiguresInitializer();
            IFiguresImagesInitializer figuresImagesInitializer = new FiguresimagesInitializerDefault();

            gameProvider = new GameProvider(figuresInitializer, figuresImagesInitializer, new Player[] { new Player("P1", Color.White), new Player("P2", Color.Black) });
        }

        #region Helpers

        private void FigurePossiblePositions(Dictionary<FigureState, List<Point>> figuresWithPossiblePositions, string assertMessage = "")
        {
            ObservableCollection<FigureState> figures = new ObservableCollection<FigureState>();
            foreach (FigureState figure in figuresWithPossiblePositions.Keys)
            {
                figures.Add(figure);
            }
            gameProvider.ResetFigures(figures);

            foreach (KeyValuePair<FigureState, List<Point>> figure in figuresWithPossiblePositions)
            {
                if (figure.Value == null)
                {
                    continue;
                }

                List<Point> positions = gameProvider.GetPossibleFigurePositions(figure.Key).ToList();

                string figureType = figure.Key.Figure.GetType().Name;
                string figureCurrentPosition = figure.Key.Position.ToString();
                StringBuilder stringBuilderPossiblePositions = new StringBuilder("");
                if (figure.Value != null)
                {
                    stringBuilderPossiblePositions.Append("Expected positions: ");
                    figure.Value.ForEach(p => stringBuilderPossiblePositions.Append($"({p.ToString()}); "));
                }
                StringBuilder stringBuilderActualPositions = new StringBuilder("Actual positions: ");
                positions.ForEach(p => stringBuilderActualPositions.Append($"({p.ToString()}); "));

                List<Point> differentPositions = figure.Value.Where(p => !positions.Any(l => p.X == l.X && p.Y == l.Y)).ToList();
                StringBuilder stringBuilderDifferencePositions = new StringBuilder("Different positions: ");
                differentPositions.ForEach(p => stringBuilderDifferencePositions.Append($"({p.ToString()}); "));

                CollectionAssert.AreEquivalent(figure.Value, positions,
                    $"{assertMessage}. {figureType} at position ({figureCurrentPosition})." +
                    $"\n{stringBuilderPossiblePositions.ToString()}." +
                    $"\n{stringBuilderActualPositions.ToString()}" +
                    $"\n{stringBuilderDifferencePositions.ToString()}");
            }
        }

        private void CheckCheckmateState(IEnumerable<FigureState> figuresWithPossiblePositions, GameProvider.CheckmateState expectedState, string assertMessage = "")
        {
            ObservableCollection<FigureState> figures = new ObservableCollection<FigureState>();
            foreach (FigureState figure in figuresWithPossiblePositions)
            {
                figures.Add(figure);
            }
            gameProvider.ResetFigures(figures);

            GameProvider.CheckmateState actualState = gameProvider.GetCheckmateState();
            Assert.AreEqual(expectedState, actualState, assertMessage);
        }

        #endregion

        [TestMethod]
        public void PawnPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false), new Point(0, 0)), new List<Point>{ new Point(0, 1), new Point(0, 2) } },
                { new FigureState(new Pawn(false), new Point(1, 1)), new List<Point>{ new Point(1, 2), new Point(1, 3) } },
                { new FigureState(new Pawn(false), new Point(2, 6)), new List<Point>{ new Point(2, 7) } },
                { new FigureState(new Pawn(false), new Point(3, 7)), new List<Point>{ } }
            }, "Game provider should correctly calculates black pawns potential positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(true), new Point(0, 7)), new List<Point>{ new Point(0, 6), new Point(0, 5) } },
                { new FigureState(new Pawn(true), new Point(1, 6)), new List<Point>{ new Point(1, 5), new Point(1, 4) } },
                { new FigureState(new Pawn(true), new Point(2, 1)), new List<Point>{ new Point(2, 0) } },
                { new FigureState(new Pawn(true), new Point(3, 0)), new List<Point>{ } }
            }, "Game provider should correctly calculates white pawns potential positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false, false), new Point(0, 0)), new List<Point>{ new Point(0, 1) } },
                { new FigureState(new Pawn(true, false), new Point(0, 7)), new List<Point>{ new Point(0, 6) } },
            }, "Pawns which made their first move should be able to move only for one cell");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false), new Point(0, 0)), new List<Point>{ new Point(0, 1) } },
                { new FigureState(new Pawn(false), new Point(0, 2)), null },
                { new FigureState(new Pawn(false), new Point(1, 0)), null },
                { new FigureState(new Pawn(false), new Point(1, 1)), null },
            }, "Pawns should not be possible to move in/through other figure");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false), new Point(0, 0), Color.Black), new List<Point>{ new Point(1, 1) } },
                { new FigureState(new Pawn(false), new Point(0, 1), Color.White), null },
                { new FigureState(new Pawn(false), new Point(1, 1), Color.White), null },
            }, "Game provider should correctly calculates pawns beat positions");
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false), new Point(1, 0), Color.Black), new List<Point>{ new Point(0, 1), new Point(2, 1) } },
                { new FigureState(new Pawn(false), new Point(0, 1), Color.White), null },
                { new FigureState(new Pawn(false), new Point(1, 1), Color.White), null },
                { new FigureState(new Pawn(false), new Point(2, 1), Color.White), null },
            }, "Game provider should correctly calculates pawns beat positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Pawn(false), new Point(1, 0), Color.Black), new List<Point>{ new Point(2, 1) } },
                { new FigureState(new Pawn(false), new Point(0, 1), Color.Black), null },
                { new FigureState(new Pawn(false), new Point(1, 1), Color.Black), null },
                { new FigureState(new Pawn(false), new Point(2, 1), Color.White), null },
            }, "Pawns should not be possible to beat their allies");
        }

        [TestMethod]
        public void RookPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Rook(), new Point(0, 0)), new List<Point>{
                    new Point(0, 1), new Point(0, 2), new Point(0, 3), new Point(0, 4), new Point(0, 5), new Point(0, 6), new Point(0, 7),
                    new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0), new Point(5, 0), new Point(6, 0), new Point(7, 0)
                } }
            }, "Game provider should correctly calculates rook potential positions");
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Rook(), new Point(5, 5)), new List<Point>{
                    new Point(4, 5), new Point(3, 5), new Point(2, 5), new Point(1, 5), new Point(0, 5),
                    new Point(6, 5), new Point(7, 5),
                    new Point(5, 4), new Point(5, 3), new Point(5, 2), new Point(5, 1), new Point(5, 0),
                    new Point(5, 6), new Point(5, 7)
                } }
            }, "Game provider should correctly calculates rook potential positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Rook(), new Point(0, 0)), new List<Point>{
                    new Point(0, 1),
                    new Point(1, 0), new Point(2, 0), new Point(3, 0)
                } },
                { new FigureState(new Rook(), new Point(0, 2)), null },
                { new FigureState(new Rook(), new Point(4, 0)), null },
            }, "Rooks should not be possible to beat their allies");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Rook(), new Point(0, 0), Color.Black), new List<Point>{
                    new Point(0, 1), new Point(0, 2),
                    new Point(1, 0), new Point(2, 0), new Point(3, 0), new Point(4, 0)
                } },
                { new FigureState(new Rook(), new Point(0, 2), Color.White), null },
                { new FigureState(new Rook(), new Point(4, 0), Color.White), null },
            }, "Rooks should not be possible to move through enemies that it can beat");
        }

        [TestMethod]
        public void KnightPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Knight(), new Point(0, 0)), new List<Point>{ new Point(1, 2), new Point(2, 1) } },
                { new FigureState(new Knight(), new Point(4, 4)), new List<Point>{ new Point(3, 2), new Point(5, 2), new Point(6, 3), new Point(6, 5), new Point(5, 6), new Point(3, 6), new Point(2, 5), new Point(2, 3) } },
            }, "Game provider should correctly calculates knights potential positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Knight(), new Point(4, 4)), new List<Point>{ new Point(3, 2), new Point(6, 3), new Point(5, 6), new Point(3, 6), new Point(2, 5), new Point(2, 3) } },
                { new FigureState(new Knight(), new Point(5, 2)), null },
                { new FigureState(new Knight(), new Point(6, 5)), null },
            }, "Knights should not be possible to beat their allies");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Knight(), new Point(0, 0), Color.Black), new List<Point>{ new Point(1, 2), new Point(2, 1) } },
                { new FigureState(new Knight(), new Point(1, 2), Color.White), null },
                { new FigureState(new Knight(), new Point(2, 1), Color.White), null },
            }, "Game provider should correctly calculates knights beat positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Knight(), new Point(4, 4)), new List<Point>{ new Point(3, 2), new Point(5, 2), new Point(6, 3), new Point(6, 5), new Point(5, 6), new Point(3, 6), new Point(2, 5), new Point(2, 3) } },
                { new FigureState(new Knight(), new Point(3, 4)), null },
                { new FigureState(new Knight(), new Point(3, 3)), null },
                { new FigureState(new Knight(), new Point(4, 3)), null },
                { new FigureState(new Knight(), new Point(5, 3)), null },
                { new FigureState(new Knight(), new Point(5, 4)), null },
                { new FigureState(new Knight(), new Point(5, 5)), null },
                { new FigureState(new Knight(), new Point(4, 5)), null },
                { new FigureState(new Knight(), new Point(3, 5)), null },
            }, "Knights shoud be possible to move through other figures");
        }

        [TestMethod]
        public void BishopPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Bishop(), new Point(0, 0)), new List<Point>{ new Point(1, 1), new Point(2, 2), new Point(3, 3), new Point(4, 4), new Point(5, 5), new Point(6, 6), new Point(7, 7) } },
                { new FigureState(new Bishop(), new Point(1, 0)), new List<Point>{ new Point(2, 1), new Point(3, 2), new Point(4, 3), new Point(5, 4), new Point(6, 5), new Point(7, 6), new Point(0, 1) } },
                { new FigureState(new Bishop(), new Point(2, 0)), new List<Point>{ new Point(3, 1), new Point(4, 2), new Point(5, 3), new Point(6, 4), new Point(7, 5), new Point(1, 1), new Point(0, 2) } },
            }, "Game provider should correctly calculates bishop potential positions");
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Bishop(), new Point(4, 4)), new List<Point>{
                    new Point(3, 3), new Point(2, 2), new Point(1, 1), new Point(0, 0),
                    new Point(5, 3), new Point(6, 2), new Point(7, 1),
                    new Point(5, 5), new Point(6, 6), new Point(7, 7),
                    new Point(3, 5), new Point(2, 6), new Point(1, 7)
                } }
            }, "Game provider should correctly calculates bishop potential positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Bishop(), new Point(4, 4), Color.Black), new List<Point>{
                    new Point(3, 3), new Point(2, 2), new Point(1, 1), new Point(0, 0),
                    new Point(5, 3), new Point(6, 2), new Point(7, 1),
                    new Point(5, 5),
                    new Point(3, 5), new Point(2, 6)
                } },
                { new FigureState(new Bishop(), new Point(5, 5), Color.White), null },
                { new FigureState(new Bishop(), new Point(2, 6), Color.White), null },
            }, "Game provider should correctly calculates bishop beat positions");
        }

        [TestMethod]
        public void QueenPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new Queen(), new Point(4, 4)), new List<Point>{
                    new Point(3, 4), new Point(2, 4), new Point(1, 4), new Point(0, 4),
                    new Point(3, 3), new Point(2, 2), new Point(1, 1), new Point(0, 0),
                    new Point(4, 3), new Point(4, 2), new Point(4, 1), new Point(4, 0),
                    new Point(5, 3), new Point(6, 2), new Point(7, 1),
                    new Point(5, 4), new Point(6, 4), new Point(7, 4),
                    new Point(5, 5), new Point(6, 6), new Point(7, 7),
                    new Point(4, 5), new Point(4, 6), new Point(4, 7),
                    new Point(3, 5), new Point(2, 6), new Point(1, 7)
                } }
            }, "Game provider should correctly calculates queens potential positions");
        }

        [TestMethod]
        public void KingPossiblePositions_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(4, 4)), new List<Point>{ new Point(3, 4), new Point(3, 3), new Point(4, 3), new Point(5, 3), new Point(5, 4), new Point(5, 5), new Point(4, 5), new Point(3, 5) } }
            }, "Game provider should correctly calculates kings potential positions");
        }

        [TestMethod]
        public void Castling_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(3, 0)), new List<Point>{ new Point(2, 0), new Point(2, 1), new Point(3, 1), new Point(4, 1), new Point(4, 0), new Point(1, 0) } },
                { new FigureState(new Rook(), new Point(0, 0)), null },
            }, "Game provider should correctly calculates left castling positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(3, 0)), new List<Point>{ new Point(2, 0), new Point(2, 1), new Point(3, 1), new Point(4, 1), new Point(4, 0), new Point(5, 0) } },
                { new FigureState(new Rook(), new Point(7, 0)), null },
            }, "Game provider should correctly calculates right castling positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(3, 0)), new List<Point>{ new Point(2, 0), new Point(2, 1), new Point(3, 1), new Point(4, 1), new Point(4, 0), new Point(1, 0), new Point(5, 0) } },
                { new FigureState(new Rook(), new Point(0, 0)), null },
                { new FigureState(new Rook(), new Point(7, 0)), null },
            }, "Game provider should correctly calculates left and right castling positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(4, 0)), new List<Point>{ new Point(3, 0), new Point(3, 1), new Point(4, 1), new Point(5, 1), new Point(5, 0) } },
            }, "Game provider should correctly calculates left and right castling positions");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(4, 0)), new List<Point>{ new Point(2, 0), new Point(3, 0), new Point(3, 1), new Point(4, 1), new Point(5, 1) } },
                { new FigureState(new Pawn(false), new Point(5, 0)),  null},
                { new FigureState(new Rook(), new Point(0, 0)), null },
                { new FigureState(new Rook(), new Point(7, 0)), null },
            }, "Game provider should correctly calculates left and right castling positions");
        }

        [TestMethod]
        public void Checkmate_Test()
        {
            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Pawn(), new Point(1, 1), Color.White),
                new FigureState(new Bishop(), new Point(4, 4), Color.Black),
            }, GameProvider.CheckmateState.None, "Without check");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Rook(), new Point(2, 0), Color.Black),
            }, GameProvider.CheckmateState.Check, "Check with rook");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Pawn(true), new Point(1, 1), Color.Black),
            }, GameProvider.CheckmateState.Check, "Check with pawn");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Knight(), new Point(1, 2), Color.Black),
            }, GameProvider.CheckmateState.Check, "Check with knight");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new Queen(), new Point(0, 0), Color.White),
                new FigureState(new King(), new Point(1, 0), Color.White),
                new FigureState(new Bishop(), new Point(2, 0), Color.White),
                new FigureState(new Knight(), new Point(3, 0), Color.White),
                new FigureState(new Rook(), new Point(4, 0), Color.White),
                new FigureState(new Pawn(), new Point(0, 1), Color.White),
                new FigureState(new Rook(), new Point(1, 1), Color.White),
                new FigureState(new Rook(), new Point(2, 3), Color.White),
                new FigureState(new Rook(), new Point(4, 1), Color.White),
                new FigureState(new Queen(), new Point(3, 2), Color.Black),
            }, GameProvider.CheckmateState.Check, "Check with queen");


            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Pawn(), new Point(0, 1), Color.White),
                new FigureState(new Pawn(), new Point(1, 1), Color.White),
                new FigureState(new Rook(), new Point(3, 0), Color.Black)
            }, GameProvider.CheckmateState.Checkmate, "Checkmate with rook and blocking pawns");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Pawn(true), new Point(0, 1), Color.Black),
                new FigureState(new Pawn(true), new Point(1, 1), Color.Black),
                new FigureState(new Pawn(true), new Point(1, 2), Color.Black),
                new FigureState(new Pawn(true), new Point(2, 2), Color.Black),
            }, GameProvider.CheckmateState.Checkmate, "Checkmate with pawns");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Rook(), new Point(1, 0), Color.Black),
                new FigureState(new Rook(), new Point(1, 1), Color.Black)
            }, GameProvider.CheckmateState.Checkmate, "Checkmate with two rooks");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(4, 0), Color.White),
                new FigureState(new Pawn(true), new Point(3, 1), Color.Black),
                new FigureState(new Pawn(true), new Point(4, 1), Color.Black),
                new FigureState(new King(), new Point(4, 2), Color.Black)
            }, GameProvider.CheckmateState.Checkmate, "Two pawn checkmate");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(6, 0), Color.White),
                new FigureState(new Rook(), new Point(5, 0), Color.White),
                new FigureState(new Pawn(), new Point(5, 1), Color.White),
                new FigureState(new Pawn(), new Point(7, 1), Color.White),
                new FigureState(new Queen(), new Point(6, 1), Color.Black),
                new FigureState(new Bishop(), new Point(0, 7), Color.Black)
            }, GameProvider.CheckmateState.Checkmate, "Diagonal checkmate");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Rook(), new Point(1, 0), Color.White),
                new FigureState(new Pawn(), new Point(0, 1), Color.White),
                new FigureState(new Pawn(), new Point(1, 1), Color.White),
                new FigureState(new Knight(), new Point(2, 1), Color.Black),
            }, GameProvider.CheckmateState.Checkmate, "Smothered checkmate");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Pawn(), new Point(0, 1), Color.White),
                new FigureState(new Bishop(), new Point(6, 6), Color.Black),
                new FigureState(new Bishop(), new Point(7, 6), Color.Black),
            }, GameProvider.CheckmateState.Checkmate, "Two bishop checkmate");

            CheckCheckmateState(new FigureState[]
            {
                new FigureState(new King(), new Point(0, 0), Color.White),
                new FigureState(new Queen(), new Point(1, 1), Color.Black),
                new FigureState(new King(), new Point(1, 2), Color.Black),
            }, GameProvider.CheckmateState.Checkmate, "Queen and king checkmate");
        }

        [TestMethod]
        public void CheckSafetyFigureMovement_Test()
        {
            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(0, 0), Color.White), null },
                { new FigureState(new Pawn(), new Point(1, 1), Color.White), new List<Point>{ } },
                { new FigureState(new Bishop(), new Point(3, 3), Color.Black), null }
            }, "Pawn should not be possible to move");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(0, 0), Color.White), null },
                { new FigureState(new Pawn(), new Point(1, 1), Color.White), new List<Point>{ new Point(2, 2) } },
                { new FigureState(new Bishop(), new Point(2, 2), Color.Black), null }
            }, "Pawn should be only possible to beat the bishop");

            FigurePossiblePositions(new Dictionary<FigureState, List<Point>>()
            {
                { new FigureState(new King(), new Point(0, 0), Color.White), null },
                { new FigureState(new Pawn(), new Point(0, 1), Color.White), null },
                { new FigureState(new Pawn(), new Point(1, 2), Color.White), null },
                { new FigureState(new Pawn(), new Point(2, 1), Color.White), new List<Point>{ new Point(2, 2) } },
                { new FigureState(new Queen(), new Point(4, 4), Color.Black), null }
            }, "Pawn should be only possible to block the king");
        }
    }
}
