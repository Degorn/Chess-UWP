using Chess_UWP.Models;
using System.Collections.Generic;
using Windows.UI;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure.Initializers
{
    public class CellsInitializer : ICellsInitializer
    {
        private readonly Color cellFirstColor = Colors.White;
        private readonly Color cellSecondColor = Colors.Gray;
        private Color currentCellColor;

        public IEnumerable<BoardCell> GetBoardCells(int boardWidth, int boardHeight)
        {
            currentCellColor = cellFirstColor;
            for (int x = 0; x < boardWidth; x++)
            {
                for (int y = 0; y < boardHeight; y++)
                {
                    yield return new BoardCell(new Point(x, y), currentCellColor);
                    ChangeCurrentCellBrushColor();
                }
                ChangeCurrentCellBrushColor();
            }
        }

        private void ChangeCurrentCellBrushColor()
        {
            currentCellColor = currentCellColor == cellFirstColor ? cellSecondColor : cellFirstColor;
        }
    }
}
