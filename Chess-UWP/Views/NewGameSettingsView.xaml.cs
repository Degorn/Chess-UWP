using Windows.UI.Xaml.Controls;

namespace Chess_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewGameSettings : Page
    {
        public NewGameSettings()
        {
            this.InitializeComponent();
        }

        private void ButtonStart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(BoardView));
        }
    }
}
