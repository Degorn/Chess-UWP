using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System.Collections.Generic;

namespace Chess_UWP.Tests.FiguresInitializators
{
    class FiguresInitializerEmpty : IFiguresInitializer
    {
        public IEnumerable<FigureState> GetFigures(IFiguresimagesInitializer imagesInitializer)
        {
            return new List<FigureState>();
        }
    }
}