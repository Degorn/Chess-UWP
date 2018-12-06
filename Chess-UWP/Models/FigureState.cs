using Caliburn.Micro;
using Chess_UWP.Models.Figures;
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
            set => NotifyOfPropertyChange(() => Position);
        }

        public Color Color { get; set; }

        private bool selected;
        public bool Selected
        {
            get => selected;
            set => NotifyOfPropertyChange(() => Selected);
        }

        public FigureState(Figure figure, Point position, Color color = Color.White)
        {
            Figure = figure;
            Position = position;
            Color = color;
        }
    }
}
