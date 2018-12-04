using System.Collections.Generic;
using Windows.Foundation;
using Chess_UWP.Infrastructure;

namespace Chess_UWP.Models.Figures
{
    public class Rook : Figure
    {
        protected override IEnumerable<Direction.Directions> Directions => new Direction.Directions[]
        {
            Direction.Directions.Bottom,
            Direction.Directions.Left,
            Direction.Directions.Right,
            Direction.Directions.Top
        };

        public override IEnumerable<Direction> PossiblePositionsToMove()
        {
            foreach (Point vector in DirectionVectors)
            {
                Direction direction = new Direction();
                int length = vector.X == 0 ? Board.BOARD_HEIGHT : Board.BOARD_WIDTH;
                for (int i = 1; i <= length; i++)
                {
                    direction.Positions.Add(vector.MultiplyBy(i));
                }
                yield return direction;
            }
        }
    }
}
