using Windows.Foundation;

namespace Chess_UWP.Models
{
    public static class Board
    {
        public const int BOARD_WIDTH = 8;
        public const int BOARD_HEIGHT = 8;

        public const int CELL_SIZE = 100;

        public enum Color
        {
            White,
            Black
        }

        private static char[] columnName = new char[8] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H' };
        public static char GetColumn(int index)
        {
            if (index < 0 && index > columnName.Length)
            {
                return '\0';
            }
            return columnName[index];
        }

        public static string GetBoardPosition(Point position)
        {
            return $"{GetColumn((int)position.X)}{position.Y}"; 
        }
    }
}