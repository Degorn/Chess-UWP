using Windows.UI.Xaml.Controls;

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

        //private string choosedType = "";
        //private async void GameProvider_PawnPromotionChoose(IEnumerable<Type> types)
        //{
        //    foreach (Type type in types)
        //    {
        //        Button newButton = new Button
        //        {
        //            Content = type.Name
        //        };
        //        StackPanelPawnPromotionSelect.Children.Add(newButton);
        //        newButton.Click += PawnPromotionTypeButton_Click;
        //    }

        //    await ContenDialogPawnPromotion.ShowAsync();
        //}

        //private void PawnPromotionTypeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    choosedType = (sender as Button).Content.ToString();
        //    gameProvider.PromotePawn(choosedType);

        //    ContenDialogPawnPromotion.Hide();
        //    StackPanelPawnPromotionSelect.Children.Clear();
        //}

        //private void Cell_PointerReleased(object sender, PointerRoutedEventArgs e)
        //{
        //    BoardCell cell = ((FrameworkElement)sender).Tag as BoardCell;
        //    Point cellPosition = cell.Position;

        //    gameProvider.DoActionByPositions(cellPosition);
        //}

        //private async void GameProvider_GameOver(object sender, GameOverEventArgs e)
        //{
        //    MessageDialog dialog = new MessageDialog($"Winner: {e.Winner.Name} ({e.Winner.Color})!", "Checkmate! Game over");
        //    await dialog.ShowAsync();
        //    Application.Current.Exit();
        //}
    }
}
