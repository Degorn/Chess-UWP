using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System.Collections.Generic;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure.GameProviderComponents
{
    public interface IFiguresHandler
    {
        FigureState CurrentlySelectedFigure { get; }

        IEnumerable<FigureState> GetFigures();
        void AddFigure(Figure figure, Point position, Color color);
        void RemoveFigure(FigureState figure);
    }
}
