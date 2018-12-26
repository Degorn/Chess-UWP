using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System.Collections.Generic;
using Windows.Foundation;

namespace Chess_UWP.Infrastructure.Initializers
{
    public interface IFiguresInitializer
    {
        IEnumerable<FigureState> GetFigures();
        FigureState GetFigure(Figure figure, Point position, Board.Color color);
    }
}