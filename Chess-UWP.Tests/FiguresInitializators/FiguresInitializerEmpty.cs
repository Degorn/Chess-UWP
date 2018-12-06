using Chess_UWP.Infrastructure.Initializers;
using Chess_UWP.Models;
using System.Collections.Generic;

namespace Chess_UWP.Tests.FiguresInitializators
{
    class FiguresInitializerEmpty : IFiguresInitializer
    {
        public IEnumerable<FigureState> GetFigures(IFiguresImagesInitializer imagesInitializer)
        {
            return new List<FigureState>();
        }
    }
}