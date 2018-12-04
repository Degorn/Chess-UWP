using Chess_UWP.Models.Figures;
using System;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure.Initializers
{
    public interface IFiguresimagesInitializer
    {
        Uri GetImage<T>(Color color) where T : Figure;
        Uri GetImage(Type type, Color color);
    }
}