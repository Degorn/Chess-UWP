using Chess_UWP.Infrastructure;
using System.Collections.Generic;
using Windows.Foundation;

namespace Chess_UWP.Models.Figures
{
    public class Bishop : Figure
    {
        protected override IEnumerable<Direction.Directions> Directions => new Direction.Directions[]
        {
            Direction.Directions.BottomLeft,
            Direction.Directions.BottomRight,
            Direction.Directions.TopLeft,
            Direction.Directions.TopRight
        };

        public override IEnumerable<Direction> PossiblePositionsToMove()
        {
            foreach (Point vector in DirectionVectors)
            {
                Direction direction = new Direction();
                for (int i = 1; i <= Board.BOARD_HEIGHT; i++)
                {
                    direction.Positions.Add(vector.MultiplyBy(i));
                }
                yield return direction;
            }
        }
    }
}
