using Windows.Foundation;
using Windows.UI;

namespace Chess_UWP.Models
{
    public class BoardCell
    {
        public int CellSize => Board.CELL_SIZE;
        public Point Position { get; set; }
        public Color Color { get; set; }

        public BoardCell(Point position, Color color)
        {
            Position = position;
            Color = color;
        }
    }
}
