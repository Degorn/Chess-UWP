using Chess_UWP.Infrastructure;
using Chess_UWP.Models;
using System.Collections.ObjectModel;

namespace Chess_UWP.ViewModels
{
    public class BoardViewModel : NotificationBase
    {
        private ObservableCollection<BoardCell> cells = new ObservableCollection<BoardCell>();
        public ObservableCollection<BoardCell> Cells
        {
            get => cells;
            set => SetProperty(ref cells, value);
        }

        private ObservableCollection<FigureState> figures = new ObservableCollection<FigureState>();
        public ObservableCollection<FigureState> Figures
        {
            get => figures;
            set => SetProperty(ref figures, value);
        }

        public BoardViewModel(ObservableCollection<FigureState> figures)
        {
            CellsInitializer cellsInitializer = new CellsInitializer();
            foreach (BoardCell item in cellsInitializer.GetBoardCells(Board.BOARD_WIDTH, Board.BOARD_HEIGHT))
            {
                cells.Add(item);
            }

            this.figures = figures;
        }
    }
}
