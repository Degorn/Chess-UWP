using Chess_UWP.Models;
using System;
using System.Threading.Tasks;

namespace Chess_UWP.Infrastructure
{
    public enum ListChagesOperation
    {
        Add, Remove
    }
    public class CollectionChangedEventHandler : EventArgs
    {
        public ListChagesOperation Operation { get; set; }
        public object Item { get; set; }
    }

    public class GameOverEventArgs : EventArgs
    {
        public Player Winner { get; set; }
        public string GameLength { get; set; }
    }

    public class TimerTickEventArgs : EventArgs
    {
        public int SecondsLeft;
    }

    public delegate void CollectionChanged(object sender, CollectionChangedEventHandler e);
    public delegate Task GameOverDelegate(object sender, GameOverEventArgs e);
    public delegate void TimerTickDelegate(object sender, TimerTickEventArgs e);
}