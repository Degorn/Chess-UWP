using System;
using Chess_UWP.Models.Figures;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure.Initializers
{
    public class FiguresimagesInitializerDefault : IFiguresImagesInitializer
    {
        public Uri GetImage<T>(Color color) where T : Figure
        {
            if (typeof(T) == typeof(Figure)) throw new ArgumentException("Type cannot be a figure.");

            string figureName = typeof(T).Name.ToLower();
            return color == Color.White ?
                new Uri($@"ms-appx:///Content/Figures/icons8-{figureName}-100.png") :
                new Uri($@"ms-appx:///Content/Figures/icons8-{figureName}-filled-100.png");
        }

        public Uri GetImage(Type type, Color color)
        {
            if (type == typeof(Figure)) throw new ArgumentException("Type cannot be a figure.");

            string figureName = type.Name.ToLower();
            return color == Color.White ?
                new Uri($@"ms-appx:///Content/Figures/icons8-{figureName}-100.png") :
                new Uri($@"ms-appx:///Content/Figures/icons8-{figureName}-filled-100.png");
        }
    }
}