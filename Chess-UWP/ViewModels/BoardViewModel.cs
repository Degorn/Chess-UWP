using Caliburn.Micro;
using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System.Collections.ObjectModel;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.ViewModels
{
    public class BoardViewModel : ViewModelBase
    {
        public GameStartSettings Parameter { get; set; }

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

        public BoardViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
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
            gameProvider.CollectionChanged += GameProvider_CollectionChanged;
            gameProvider.StartPawnPromotion += StartPawnPromition;
            gameProvider.GameOver += GameOver;

            Figures = new ObservableCollection<FigureState>(gameProvider.GetFigures());
            PawnPromotionTypes = new ObservableCollection<string>(gameProvider.GetPawnPromotionTypes());
        }

        private void GameProvider_CollectionChanged(object sender, CollectionChangedEventHandler e)
        {
            switch (e.Operation)
            {
                case ListChagesOperation.Add:
                    Figures.Add((FigureState)e.Item);
                    break;
                case ListChagesOperation.Remove:
                    Figures.Remove((FigureState)e.Item);
                    break;
                default:
                    break;
            }
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
