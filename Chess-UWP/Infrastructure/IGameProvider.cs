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
        IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool includeCheckmateState = true);
        void DoActionByPositions(Point position);
        void ResetFigures(IEnumerable<FigureState> figures);

        CheckmateState GetCheckmateState();

        void PromotePawn(PawnPromotionType type);

        void SetMoveTimer(int secondsOnMove);
        void StartMoveTimer();
    }
}