using static Chess_UWP.Models.Board;

namespace Chess_UWP.Models
{
    public class Player
    {
        //public enum PlayerColor
        //{
        //    White,
        //    Black
        //}

        public string Name { get; set; }
        public Color Color { get; set; }

        public Player(string name, Color color)
        {
            Name = name;
            Color = color;
        }
    }
}
