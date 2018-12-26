using System.Collections.Generic;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public interface IGameProvider
    {
        event CollectionChanged CollectionChanged;
        event GameOverDelegate GameOver;
        event PawnPromotionDelegate StartPawnPromotion;
        event TimerTickDelegate TimerTick;
        event MoveLogDelegate LogMove;

        FigureState CurrentlySelectedFigure { get; }
        IEnumerable<FigureState> GetFigures();
        void ResetFigures(IEnumerable<FigureState> figures);
        void DoActionByPositions(Point position);
        IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool includeCheckmateState = true);

        GameProvider.CheckmateState GetCheckmateState();
        void PromotePawn(string type);

        void SetMoveTimer(int secondsOnMove);
        void StartMoveTimer();
    }
}