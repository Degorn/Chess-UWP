using System;
using System.Collections.Generic;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public interface IGameProvider
    {
        event CollectionChanged CollectionChanged;
        event GameOverDelegate GameOver;
        event EventHandler StartPawnPromotion;
        event TimerTickDelegate TimerTick;

        IEnumerable<FigureState> GetFigures();
        void ResetFigures(IEnumerable<FigureState> figures);
        void DoActionByPositions(Point position);
        IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool includeCheckmateState);

        GameProvider.CheckmateState GetCheckmateState();
        IEnumerable<string> GetPawnPromotionTypes();
        void PromotePawn(string type);

        void SetMoveTimer(int secondsOnMove);
        void StartMoveTimer();
    }
}