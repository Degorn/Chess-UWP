using Windows.UI.Xaml.Controls;
using System;
using Chess_UWP.ViewModels;
using Chess_UWP.Infrastructure;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace Chess_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BoardView : Page
    {
        public BoardView()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            BoardViewModel viewModel = DataContext as BoardViewModel;
            viewModel.StartPawnPromotion += ShowPawnPromotionDialog;
            viewModel.EndPawnPromotion += ClosePawnPromotionDialog;
            viewModel.GameOverEvent += GameOver;
        }

        private async void ShowPawnPromotionDialog()
        {
            await ContenDialogPawnPromotion.ShowAsync();
        }

        private void ClosePawnPromotionDialog()
        {
            ContenDialogPawnPromotion.Hide();
        }

        private async void GameOver(object sender, GameOverEventArgs e)
        {
            MessageDialog dialog = new MessageDialog($"Winner: {e.Winner.Name} ({e.Winner.Color})!", "Checkmate! Game over");
            await dialog.ShowAsync();
            Application.Current.Exit();
        }

        private void ButtonPawnPromotion_Click(object sender, RoutedEventArgs e)
        {
            BoardViewModel viewModel = DataContext as BoardViewModel;
            string choosedType = (sender as Button).Content.ToString();
            viewModel.PawnPromotion(choosedType);
        }
    }
}
