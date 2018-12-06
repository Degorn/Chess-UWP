using Caliburn.Micro;
using Chess_UWP.Models;

namespace Chess_UWP.ViewModels
{
    public class NewGameSettingsViewModel : ViewModelBase
    {
        private bool isTurnLimit;
        public bool IsTurnLimit
        {
            get => isTurnLimit;
            set
            {
                isTurnLimit = value;
                NotifyOfPropertyChange(() => IsTurnLimit);
            }
        }

        private INavigationService pageNavigationService;
        public NewGameSettingsViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
            this.pageNavigationService = pageNavigationService;
        }

        private void Start(string firstUserName, string secondUserName, int secondsOnTurn)
        {
            if (!IsTurnLimit)
            {
                secondsOnTurn = 0;
            }
            pageNavigationService.NavigateToViewModel<BoardViewModel>(new GameStartSettings
            {
                FirstUserName = firstUserName ?? "Player 1",
                SecondUserName = secondUserName ?? "Player 2",
                SecondsOnTurn = secondsOnTurn
            });
        }
    }
}
