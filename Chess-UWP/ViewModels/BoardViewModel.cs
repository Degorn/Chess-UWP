using Caliburn.Micro;
using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using static Chess_UWP.Models.Board;
using System.Linq;
using Chess_UWP.Database;

namespace Chess_UWP.ViewModels
{
    public class BoardViewModel : ViewModelBase
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

        private ObservableCollection<FigureViewModel> figures = new ObservableCollection<FigureViewModel>();
        public ObservableCollection<FigureViewModel> Figures
        {
            get => figures;
            set
            {
                figures = value;
                NotifyOfPropertyChange(() => Figures);
            }
        }

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

        private bool isPawnPromotion;
        public bool IsPawnPromotion
        {
            get => isPawnPromotion;
            set
            {
                isPawnPromotion = value;
                NotifyOfPropertyChange(() => IsPawnPromotion);
            }
        }

        private string timer;
        public string Timer
        {
            get => timer;
            set
            {
                timer = value;
                NotifyOfPropertyChange(() => Timer);
            }
        }

        public GameStartSettings Parameter { get; set; }
        private IGameProvider gameProvider;
        private IRepository repository;

        public BoardViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
            CellsInitializer cellsInitializer = new CellsInitializer();
            foreach (BoardCell item in cellsInitializer.GetBoardCells(BOARD_WIDTH, BOARD_HEIGHT))
            {
                cells.Add(item);
            }
        }

        protected override void OnActivate()
        {
            Player playerWhite = new Player(string.IsNullOrEmpty(Parameter?.FirstUserName) ? "Player 1" : Parameter.FirstUserName, Color.White);
            Player playerBlack = new Player(string.IsNullOrEmpty(Parameter?.SecondUserName) ? "Player 2" : Parameter.SecondUserName, Color.Black);
            IFiguresInitializer figuresInitializer = IoC.Get<IFiguresInitializer>();
            IFiguresImagesInitializer figuresImagesInitializer = IoC.Get<IFiguresImagesInitializer>();

            gameProvider = new GameProvider(figuresInitializer, figuresImagesInitializer, new Player[] { playerWhite, playerBlack });
            gameProvider.CollectionChanged += CollectionChanged;
            gameProvider.StartPawnPromotion += StartPawnPromition;
            gameProvider.GameOver += GameOver;

            Figures = new ObservableCollection<FigureViewModel>();
            IEnumerable<FigureState> figures = gameProvider.GetFigures();
            foreach (FigureState figure in figures)
            {
                Figures.Add(new FigureViewModel
                {
                    FigureState = figure
                });
            }
            PawnPromotionTypes = new ObservableCollection<string>(gameProvider.GetPawnPromotionTypes());

            gameProvider.SetMoveTimer(Parameter?.SecondsOnTurn ?? 0);
            gameProvider.TimerTick += TimerTick;
            gameProvider.StartMoveTimer();

            repository = IoC.Get<IRepository>();
        }

        private void TimerTick(object sender, TimerTickEventArgs e)
        {
            Timer += $"\n{e.SecondsLeft} seconds left;";
        }

        private void CollectionChanged(object sender, CollectionChangedEventHandler e)
        {
            switch (e.Operation)
            {
                case ListChagesOperation.Add:
                    Figures.Add(new FigureViewModel()
                    {
                        FigureState = (FigureState)e.Item
                    });
                    break;
                case ListChagesOperation.Remove:
                    Figures.Remove(Figures.First(f => f.FigureState == (FigureState)e.Item));
                    break;
                default:
                    break;
            }
        }

        public event GameOverDelegate GameOverEvent;

        private void CellClick(BoardCell cell)
        {
            Point cellPosition = cell.Position;
            gameProvider.DoActionByPositions(cellPosition);
        }

        private void StartPawnPromition(object sender, EventArgs e)
        {
            IsPawnPromotion = true;
        }

        public void PawnPromotion(string type)
        {
            gameProvider.PromotePawn(type);
            IsPawnPromotion = false;
        }

        private void GameOver(object sender, GameOverEventArgs e)
        {
            GameOverEvent(sender, e);
            repository.AddAsync(new GameInfo
            {
                FirstPlayerName = "",
                SecondPlayerName = "",
                GameLength = e.GameLength,
                Winner = e.Winner.Name
            });
        }
    }
}
