using Chess_UWP.Models;
using System;

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
    }

    public delegate void CollectionChanged(object sender, CollectionChangedEventHandler e);
    public delegate void UserInputDelegate();
    public delegate void GameOverDelegate(object sender, GameOverEventArgs e);
}