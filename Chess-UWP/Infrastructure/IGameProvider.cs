using System.Collections.Generic;
using Chess_UWP.Infrastructure.GameProviderComponents;
using Chess_UWP.Infrastructure.GameProviderComponents.PlayersContainer;
using Chess_UWP.Models;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure
{
    public interface IGameProvider : IMotionHandler, IPlayersContainer, IGameStartAndOver, IFiguresHandler
    {
        event CollectionChanged CollectionChanged;

        IEnumerable<Point> GetPossibleFigurePositions(FigureState figure, bool includeCheckmateState = true);
        void DoActionByPositions(Point position);
        void ResetFigures(IEnumerable<FigureState> figures);

        CheckmateState GetCheckmateState();
    }
}