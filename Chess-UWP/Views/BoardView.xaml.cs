using Chess_UWP.Infrastructure;
using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using Chess_UWP.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using static Chess_UWP.Models.Board;
using System.Collections.Generic;
using System;
using Windows.UI.Popups;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Chess_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BoardView : Page
    {
        private BoardViewModel boardViewModel;
        private GameProvider gameProvider;

        private Player playerWhite = new Player("Player1", Color.White);
        private Player playerBlack = new Player("Player2", Color.Black);

        public BoardView()
        {
            this.InitializeComponent();

            IFiguresInitializer figuresInitializer = new FiguresInitializer();
            IFiguresimagesInitializer figuresImagesInitializer = new FiguresimagesInitializerDefault();

            gameProvider = new GameProvider(figuresInitializer, figuresImagesInitializer, new Player[] { playerWhite, playerBlack });
            boardViewModel = new BoardViewModel(gameProvider.FiguresOnBoard);
            gameProvider.StartPawnPromotion += GameProvider_PawnPromotionChoose;
            gameProvider.GameOver += GameProvider_GameOver;
        }

        private string choosedType = "";
        private async void GameProvider_PawnPromotionChoose(IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                Button newButton = new Button
                {
                    Content = type.Name
                };
                StackPanelPawnPromotionSelect.Children.Add(newButton);
                newButton.Click += PawnPromotionTypeButton_Click;
            }

            await ContenDialogPawnPromotion.ShowAsync();
        }

        private void PawnPromotionTypeButton_Click(object sender, RoutedEventArgs e)
        {
            choosedType = (sender as Button).Content.ToString();
            gameProvider.PromotePawn(choosedType);

            ContenDialogPawnPromotion.Hide();
            StackPanelPawnPromotionSelect.Children.Clear();
        }

        private void Cell_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            BoardCell cell = ((FrameworkElement)sender).Tag as BoardCell;
            Point cellPosition = cell.Position;

            gameProvider.DoActionByPositions(cellPosition);
        }

        private async void GameProvider_GameOver(object sender, GameOverEventArgs e)
        {
            MessageDialog dialog = new MessageDialog($"Winner: {e.Winner.Name} ({e.Winner.Color})!", "Checkmate! Game over");
            await dialog.ShowAsync();
            Application.Current.Exit();
        }
    }
}
