using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Chess_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenuView : Page
    {
        public MainMenuView()
        {
            this.InitializeComponent();
        }

        private void ButtonNewGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewGameSettings));
        }

        private void ButtonSettings_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(SettingsView));
        }

        private void ButtonExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
