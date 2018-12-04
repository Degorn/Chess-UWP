using Chess_UWP.Models;
using System.Collections.Generic;

namespace Chess_UWP.Infrastructure.Initializers
{
    public interface IFiguresInitializer
    {
        IEnumerable<FigureState> GetFigures(IFiguresimagesInitializer imagesInitializer);
    }
}