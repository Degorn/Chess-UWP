using Chess_UWP.Models.Figures;
using Chess_UWP.ViewModels;
using Windows.Foundation;
using Windows.UI.Xaml;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Models
{
    public class FigureState : NotificationBase
    {
        public Figure Figure { get; set; }

        private Point position;
        public Point Position
        {
            get => position;
            set => SetProperty(ref position, value);
        }

        public Color Color { get; set; }

        private bool selected;
        public bool Selected
        {
            get => selected;
            set => SetProperty(ref selected, value);
        }

        public FigureState(Figure figure, Point position, Color color = Color.White)
        {
            Figure = figure;
            this.position = position;
            Color = color;
        }
    }
}
