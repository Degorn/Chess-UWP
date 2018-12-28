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
using System.Threading.Tasks;
using Windows.UI.Popups;
using Chess_UWP.Infrastructure.GameProviderComponents.MoveTimer;
using Chess_UWP.Infrastructure.GameProviderComponents.PlayersContainer;
using Chess_UWP.Infrastructure.GameProviderComponents;
using Chess_UWP.Infrastructure.GameProviderComponents.Logger;
using Chess_UWP.Infrastructure.GameProviderComponents.GameTimer;

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

        private ObservableCollection<PawnPromotionType> pawnPromotionTypes = new ObservableCollection<PawnPromotionType>();
        public ObservableCollection<PawnPromotionType> PawnPromotionTypes
        {
            get => pawnPromotionTypes;
            set
            {
                pawnPromotionTypes = value;
                NotifyOfPropertyChange(() => PawnPromotionTypes);
            }
        }

        private ObservableCollection<Move> moves = new ObservableCollection<Move>();
        public ObservableCollection<Move> Moves
        {
            get => moves;
            set
            {
                moves = value;
                NotifyOfPropertyChange(() => Moves);
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

        private ObservableCollection<PossiblePosition> figurePositions = new ObservableCollection<PossiblePosition>();
        public ObservableCollection<PossiblePosition> FigurePositions
        {
            get => figurePositions;
            set
            {
                figurePositions = value;
                NotifyOfPropertyChange(() => FigurePositions);
            }
        }

        INavigationService pageNavigationService;

        public GameStartSettings Parameter { get; set; }
        private IGameProvider gameProvider;
        private IMoveTimer moveTimer;
        private IRepository repository;
        string playerWhite, playerBlack;

        public BoardViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
            this.pageNavigationService = pageNavigationService;

            CellsInitializer cellsInitializer = new CellsInitializer();
            foreach (BoardCell item in cellsInitializer.GetBoardCells(BOARD_WIDTH, BOARD_HEIGHT))
            {
                cells.Add(item);
            }
        }

        protected override void OnActivate()
        {
            gameProvider = IoC.Get<IGameProvider>();
            gameProvider.CollectionChanged += CollectionChanged;
            gameProvider.StartPawnPromotion += StartPawnPromition;
            gameProvider.GameStart += GameProvider_GameStart;

            Figures = new ObservableCollection<FigureViewModel>();
            IEnumerable<FigureState> figures = gameProvider.GetFigures();
            foreach (FigureState figure in figures)
            {
                Figures.Add(new FigureViewModel
                {
                    FigureState = figure
                });
            }

            IGameTimer gameTimer = new GameTimer(gameProvider.Instance);

            moveTimer = new MoveTimer(IoC.Get<IGameProvider>());
            moveTimer.SetTimer(Parameter?.SecondsOnTurn ?? 0);
            moveTimer.TimerTick += TimerTick;
            moveTimer.TimeIsUp += MoveTimer_TimeIsUp;

            IMotionHandler motionHandler = IoC.Get<IGameProvider>();
            motionHandler.Move += GameProvider_LogMove;

            playerWhite = string.IsNullOrEmpty(Parameter?.FirstUserName) ? "Player 1" : Parameter.FirstUserName;
            playerBlack = string.IsNullOrEmpty(Parameter?.SecondUserName) ? "Player 2" : Parameter.SecondUserName;
            IPlayersContainer playersContainer = IoC.Get<IGameProvider>();
            playersContainer.SetPlayers(playerWhite, playerBlack);

            ILogger logger = new Logger(gameProvider.Instance, gameTimer);
            logger.GameOver += GameOverAsync;

            repository = IoC.Get<IRepository>();
        }

        private void GameProvider_GameStart(object sender, EventArgs e)
        {
            moveTimer.StartTimer();
        }

        private void MoveTimer_TimeIsUp(object sender, EventArgs e)
        {
            FigurePositions.Clear();
        }

        private void GameProvider_LogMove(object sender, MoveEventArgs e)
        {
            Moves.Add(new Move
            {
                Figure = e.Figure,
                Color = e.Color,
                StartPosition = e.StartPosition,
                EndPosition = e.EndPosition
            });
        }

        private void TimerTick(object sender, TimerTickEventArgs e)
        {
            Timer = $"Seconds left: {e.SecondsLeft}";
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

        private void CellClick(BoardCell cell)
        {
            Point cellPosition = cell.Position;
            gameProvider.DoActionByPositions(cellPosition);

            FigurePositions.Clear();
            if (gameProvider.CurrentlySelectedFigure != null)
            {
                foreach (Point position in gameProvider.GetPossibleFigurePositions(gameProvider.CurrentlySelectedFigure))
                {
                    FigurePositions.Add(new PossiblePosition
                    {
                        Position = position
                    });
                }
            }
        }

        private void StartPawnPromition(object sender, PawnPromotionEventArgs e)
        {
            PawnPromotionTypes = new ObservableCollection<PawnPromotionType>(e.Types);
            IsPawnPromotion = true;
        }

        public void PawnPromotion(PawnPromotionType type)
        {
            gameProvider.PromotePawn(type);
            IsPawnPromotion = false;
        }

        private async void GameOverAsync(object sender, GameOverEventArgs e)
        {
            await ShowGameOverDialogAsync(e);
            await SaveResultAsync(e);

            moveTimer.StopTimer();

            pageNavigationService.NavigateToViewModel<MainMenuViewModel>();
        }

        private async Task ShowGameOverDialogAsync(GameOverEventArgs e)
        {
            MessageDialog dialog = new MessageDialog($"Winner: {e.Winner.Name} ({e.Winner.Color})!\nGameLength: {e.GameLength}", "Checkmate! Game over");
            await dialog.ShowAsync();
        }

        private async Task SaveResultAsync(GameOverEventArgs e)
        {
            await repository.AddAsync(new GameInfo
            {
                FirstPlayerName = playerWhite,
                SecondPlayerName = playerBlack,
                GameLength = e.GameLength,
                Winner = e.Winner.Name,
                Date = DateTime.Now
            });
        }
    }
}
