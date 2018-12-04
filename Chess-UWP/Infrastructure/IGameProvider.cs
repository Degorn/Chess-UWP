using System.Collections.Generic;
using System.Collections.ObjectModel;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public interface IGameProvider
    {
        ObservableCollection<FigureState> FiguresOnBoard { get; }

        event GameProvider.GameOverDelegate GameOver;
        event GameProvider.UserInputDelegate StartPawnPromotion;

        void DoActionByPositions(Point position);
        GameProvider.CheckmateState GetCheckmateState();
        IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool isPotentialCalculation = false);
        void PromotePawn(string type);
        void ResetFigures(IEnumerable<FigureState> figures);
        void ResetFigures(ObservableCollection<FigureState> figures);
    }
}