using Chess_UWP.Models;
using System;
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

            CreteCellDesignation();
        }

        private void CreteCellDesignation()
        {
            for (int i = 0; i < Board.BOARD_HEIGHT; i++)
            {
                TextBlock newTextBlock = new TextBlock
                {
                    Text = (Board.BOARD_HEIGHT - i).ToString()
                };
                RowDesignaion.RowDefinitions.Add(new RowDefinition());
                RowDesignaion.Children.Add(newTextBlock);
                Grid.SetRow(newTextBlock, i);
            }
            for (int i = 0; i < Board.BOARD_WIDTH; i++)
            {
                TextBlock newTextBlock = new TextBlock
                {
                    Text = (Board.GetColumn(i)).ToString()
                };
                ColumnDesignaion.ColumnDefinitions.Add(new ColumnDefinition());
                ColumnDesignaion.Children.Add(newTextBlock);
                Grid.SetColumn(newTextBlock, i);
            }
        }
    }
}
