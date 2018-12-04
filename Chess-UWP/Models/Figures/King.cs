using Chess_UWP.Infrastructure;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Media;

namespace Chess_UWP.Models.Figures
{
    public class King : Figure
    {
        protected override IEnumerable<Direction.Directions> Directions => new Direction.Directions[]
        {
            Direction.Directions.BottomLeft,
            Direction.Directions.BottomRight,
            Direction.Directions.TopLeft,
            Direction.Directions.TopRight,
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
                direction.Positions.Add(vector);
                yield return direction;
            }
        }
    }
}
