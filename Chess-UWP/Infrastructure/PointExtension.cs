using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public static class PointExtension
    {
        public static Point Add(this Point p1, Point p2)
        {
            return new Point
            {
                X = p1.X + p2.X,
                Y = p1.Y + p2.Y
            };
        }

        public static Point Subtract(this Point p1, Point p2)
        {
            return new Point
            {
                X = p1.X - p2.X,
                Y = p1.Y - p2.Y
            };
        }

        public static Point MultiplyBy(this Point p1, int number)
        {
            return new Point
            {
                X = p1.X * number,
                Y = p1.Y * number
            };
        }

        public static bool CheckIfOutsideTheBoard(this Point p)
        {
            return (p.X < 0 || p.X > Board.BOARD_WIDTH - 1 ||
                    p.Y < 0 || p.Y > Board.BOARD_HEIGHT - 1);
        }
    }
}