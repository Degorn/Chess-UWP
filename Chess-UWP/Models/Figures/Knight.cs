using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Chess_UWP.Models.Figures
{
    public class Knight : Figure
    {
        protected override IEnumerable<Direction.Directions> Directions => new Direction.Directions[]
        {
            Direction.Directions.None
        };

        public override IEnumerable<Direction> PossiblePositionsToMove()
        {
            Point[] additionalPoints = 
            {
                new Point(2, -1),
                new Point(2, 1),
                new Point(-2, -1),
                new Point(-2, 1),
                new Point(1, 2),
                new Point(-1, 2),
                new Point(1, -2),
                new Point(-1, -2),
            };

            foreach (Point point in additionalPoints)
            {
                Direction direction = new Direction();
                direction.Positions.Add(point);
                yield return direction;
            }
        }
    }
}
