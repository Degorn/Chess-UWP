using Caliburn.Micro;
using Chess_UWP.Views;
using Windows.UI.Xaml;

namespace Chess_UWP.ViewModels
{
    public class MainMenuViewModel : ViewModelBase
    {
        public MainMenuViewModel(INavigationService pageNavigationService) : base(pageNavigationService)
        {
        }

        private void NewGame()
        {
            NavigateTo<NewGameSettingsView>();
        }

        private void Settings()
        {
            NavigateTo<SettingsView>();
        }

        private void Close()
        {
            Application.Current.Exit();
        }
    }
}
