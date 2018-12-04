using Chess_UWP.Infrastructure;
using System.Collections.Generic;
using Windows.Foundation;

namespace Chess_UWP.Models.Figures
{
    public class Pawn : Figure
    {
        protected override IEnumerable<Direction.Directions> Directions { get; }

        public Pawn(bool isDirectedUp = false, bool isFirstStep = true)
        {
            if (isDirectedUp)
            {
                Directions = new Direction.Directions[]
                {
                    Direction.Directions.TopLeft,
                    Direction.Directions.Top,
                    Direction.Directions.TopRight
                };
            }
            else
            {
                Directions = new Direction.Directions[]
                {
                    Direction.Directions.BottomLeft,
                    Direction.Directions.Bottom,
                    Direction.Directions.BottomRight
                };
            }

            IsNeverMoved = isFirstStep;
            SetDirectionVectors();
        }

        public override IEnumerable<Direction> PossiblePositionsToMove()
        {
            foreach (Point vector in DirectionVectors)
            {
                Direction direction = new Direction();
                int x = IsNeverMoved && vector.X == 0 ? 2 : 1; // First step and directed forward.
                for (int i = 1; i <= x; i++)
                {
                    direction.Positions.Add(vector.MultiplyBy(i));
                }
                yield return direction;
            }
        }
    }
}