using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public enum CheckmateState
    {
        None, Check, Checkmate
    }

    public enum ListChagesOperation
    {
        Add, Remove
    }

    public enum PawnPromotionType
    {
        Rook, Knight, Bishop, Queen
    }

    public class CollectionChangedEventHandler : EventArgs
    {
        public ListChagesOperation Operation { get; set; }
        public object Item { get; set; }
    }
    public delegate void CollectionChanged(object sender, CollectionChangedEventHandler e);

    public class GameOverEventArgs : EventArgs
    {
        public Player Winner { get; set; }
        public string GameLength { get; set; }
    }
    public delegate Task GameOverDelegate(object sender, GameOverEventArgs e);

    public class TimerTickEventArgs : EventArgs
    {
        public int SecondsLeft { get; set; }
    }
    public delegate void TimerTickDelegate(object sender, TimerTickEventArgs e);

    public class MoveLogEventArgs : EventArgs
    {
        public Figure Figure { get; set; }
        public Board.Color Color { get; set; }
        public Point StartPosition { get; set; }
        public Point EndPosition { get; set; }
    }
    public delegate void MoveLogDelegate(object sender, MoveLogEventArgs e);

    public class PawnPromotionEventArgs : EventArgs
    {
        public IEnumerable<PawnPromotionType> Types;
    }
    public delegate void PawnPromotionDelegate(object sender, PawnPromotionEventArgs e);
}