using Chess_UWP.Models.Figures;
using Windows.Foundation;

namespace Chess_UWP.Models
{
    public class Move
    {
        public Figure Figure { get; set; }
        public Board.Color Color { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }

        public string Result
        {
            get
            {
                return $"({Color.ToString()[0]}) {Figure.GetType().Name.ToString()[0]} {StartPosition} -> {EndPosition}";
            }
        }
    }
}