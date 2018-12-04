using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace Chess_UWP.Models.Figures
{
    public abstract class Figure
    {
        public Uri Image { get; set; }

        protected abstract IEnumerable<Direction.Directions> Directions { get; }
        protected ICollection<Point> DirectionVectors { get; } = new List<Point>();

        public bool IsNeverMoved { get; protected set; } = true;

        public Figure()
        {
            SetDirectionVectors();
        }

        protected void SetDirectionVectors()
        {
            if (Directions == null)
            {
                return; 
            }

            foreach (Direction.Directions direction in Directions)
            {
                DirectionVectors.Add(Direction.GetVector(direction));
            }
        }

        public abstract IEnumerable<Direction> PossiblePositionsToMove();

        public void Step()
        {
            IsNeverMoved = false;
        }
    }
}