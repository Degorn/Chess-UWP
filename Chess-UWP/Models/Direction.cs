using System.Collections.Generic;
using Windows.Foundation;

namespace Chess_UWP.Models
{
    public class Direction
    {
        public enum Directions
        {
            None,
            Left,
            TopLeft,
            Top,
            TopRight,
            Right,
            BottomRight,
            Bottom,
            BottomLeft
        }

        private static readonly Dictionary<Directions, Point> Vectors = new Dictionary<Directions, Point>
        {
            { Directions.None, new Point(0, 0) },
            { Directions.Left, new Point(-1, 0) },
            { Directions.TopLeft, new Point(-1, -1) },
            { Directions.Top, new Point(0, -1) },
            { Directions.TopRight, new Point(1, -1) },
            { Directions.Right, new Point(1, 0) },
            { Directions.BottomRight, new Point(1, 1) },
            { Directions.Bottom, new Point(0, 1) },
            { Directions.BottomLeft, new Point(-1, 1) },
        };

        public Point StartPosition { get; set; }
        public ICollection<Point> Positions { get; set; } = new List<Point>();

        public static Point GetVector(Directions direction)
        {
            if (Vectors.TryGetValue(direction, out Point vector))
            {
                return vector;
            }
            else
            {
                throw new KeyNotFoundException($"The key {nameof(direction)} does not exist id {Vectors.GetType()}{nameof(Vectors)}");
            }
        }

        public override string ToString()
        {
            return string.Join("; ", Positions);
        }
    }
}
