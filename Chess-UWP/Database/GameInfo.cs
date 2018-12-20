using System;

namespace Chess_UWP.Database
{
    public class GameInfo
    {
        public int Id { get; set; }
        public string FirstPlayerName { get; set; }
        public string SecondPlayerName { get; set; }
        public string GameLength { get; set; }
        public string Winner { get; set; }
        public DateTime Date { get; set; }
    }
}