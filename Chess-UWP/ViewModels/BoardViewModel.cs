using Caliburn.Micro;
using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.ViewModels
{
    public class BoardViewModel : Screen
    {
        private ObservableCollection<BoardCell> cells = new ObservableCollection<BoardCell>();
        public ObservableCollection<BoardCell> Cells
        {
            get => cells;
            set
            {
                cells = value;
                NotifyOfPropertyChange(() => Cells);
            }
        }

        private ObservableCollection<FigureState> figures = new ObservableCollection<FigureState>();
        public ObservableCollection<FigureState> Figures
        {
            get => figures;
            set
            {
                figures = value;
                NotifyOfPropertyChange(() => Figures);
            }
        }

        IGameProvider gameProvider;

        public BoardViewModel()
        {
            CellsInitializer cellsInitializer = new CellsInitializer();
            foreach (BoardCell item in cellsInitializer.GetBoardCells(Board.BOARD_WIDTH, Board.BOARD_HEIGHT))
            {
                cells.Add(item);
            }

            Player playerWhite = new Player("Player1", Color.White);
            Player playerBlack = new Player("Player2", Color.Black);
            IFiguresInitializer figuresInitializer = new FiguresInitializer();
            IFiguresimagesInitializer figuresImagesInitializer = new FiguresimagesInitializerDefault();
            gameProvider = new GameProvider(figuresInitializer, figuresImagesInitializer, new Player[] { playerWhite, playerBlack });

            //gameProvider.StartPawnPromotion += GameProvider_PawnPromotionChoose;
            //gameProvider.GameOver += GameProvider_GameOver;

            Figures = gameProvider.FiguresOnBoard;
        }

        private void Cell(BoardCell cell)
        {
            Point cellPosition = cell.Position;
            gameProvider.DoActionByPositions(cellPosition);
        }
    }
}
