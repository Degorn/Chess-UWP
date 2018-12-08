using Caliburn.Micro;
using Chess_UWP.Models.Figures;
using System;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Models
{
    public class FigureState : PropertyChangedBase
    {
        public Figure Figure { get; set; }

        private Point position;
        public Point Position
        {
            get => position;
            set
            {
                PositionChanged?.Invoke(this, EventArgs.Empty);
                position = value;
                NotifyOfPropertyChange(() => Position);
                NotifyOfPropertyChange(() => X);
                NotifyOfPropertyChange(() => Y);
            }
        }

        public double X => Position.X * 100;
        public double Y => Position.Y * 100;

        public Color Color { get; set; }

        private bool selected;
        public bool Selected
        {
            get => selected;
            set
            {
                selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
        }

        public FigureState(Figure figure, Point position, Color color = Color.White)
        {
            Figure = figure;
            Position = position;
            Color = color;
        }

        public event EventHandler PositionChanged;
    }
}
