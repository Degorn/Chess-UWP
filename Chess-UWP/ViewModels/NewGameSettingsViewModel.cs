using Caliburn.Micro;
using Chess_UWP.Models;

namespace Chess_UWP.ViewModels
{
    public class NewGameSettingsViewModel : ViewModelBase
    {
        private INavigationService pageNavigationService;
        public NewGameSettingsViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
            this.pageNavigationService = pageNavigationService;
        }

        private void Start(string firstUserName, string secondUserName, int secondsOnTurn)
        {
            pageNavigationService.NavigateToViewModel<BoardViewModel>(new GameStartSettings
            {
                FirstUserName = firstUserName,
                SecondUserName = secondUserName,
                SecondsOnTurn = secondsOnTurn
            });
        }
    }
}
