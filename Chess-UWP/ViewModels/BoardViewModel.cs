using Caliburn.Micro;
using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System.Collections.ObjectModel;
using Windows.Foundation;
using static Chess_UWP.Infrastructure.GameProvider;
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

        private IGameProvider gameProvider;

        private ObservableCollection<string> pawnPromotionTypes = new ObservableCollection<string>();
        public ObservableCollection<string> PawnPromotionTypes
        {
            get => pawnPromotionTypes;
            set
            {
                pawnPromotionTypes = value;
                NotifyOfPropertyChange(() => PawnPromotionTypes);
            }
        }

        public BoardViewModel()
        {
            CellsInitializer cellsInitializer = new CellsInitializer();
            foreach (BoardCell item in cellsInitializer.GetBoardCells(BOARD_WIDTH, BOARD_HEIGHT))
            {
                cells.Add(item);
            }

            Player playerWhite = new Player("Player1", Color.White);
            Player playerBlack = new Player("Player2", Color.Black);
            IFiguresInitializer figuresInitializer = new FiguresInitializer();
            IFiguresimagesInitializer figuresImagesInitializer = new FiguresimagesInitializerDefault();
            gameProvider = new GameProvider(figuresInitializer, figuresImagesInitializer, new Player[] { playerWhite, playerBlack });
            PawnPromotionTypes = new ObservableCollection<string>(gameProvider.GetPawnPromotionTypes());

            gameProvider.StartPawnPromotion += StartPawnPromition;
            gameProvider.GameOver += GameOver;

            Figures = gameProvider.FiguresOnBoard;
        }

        public event UserInputDelegate StartPawnPromotion;
        public event UserInputDelegate EndPawnPromotion;
        public event GameOverDelegate GameOverEvent;

        private void CellClick(BoardCell cell)
        {
            Point cellPosition = cell.Position;
            gameProvider.DoActionByPositions(cellPosition);
        }

        private void StartPawnPromition()
        {
            StartPawnPromotion();
        }

        public void PawnPromotion(string type)
        {
            gameProvider.PromotePawn(type);
            EndPawnPromotion();
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            GameOverEvent(sender, e);
        }
    }
}
