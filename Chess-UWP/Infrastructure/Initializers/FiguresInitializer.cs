using Chess_UWP.Models;
using Chess_UWP.Models.Figures;
using System.Collections.Generic;
using Windows.Foundation;
using static Chess_UWP.Models.Board;

namespace Chess_UWP.Infrastructure.Initializers
{
    public class FiguresInitializer : IFiguresInitializer
    {
        private readonly IFiguresImagesInitializer imagesInitializer;

        public FiguresInitializer(IFiguresImagesInitializer imagesInitializer)
        {
            this.imagesInitializer = imagesInitializer;
        }

        public IEnumerable<FigureState> GetFigures()
        {
            List<FigureState> figures = new List<FigureState>
            {
                new FigureState(new Rook{ Image = imagesInitializer?.GetImage<Rook>(Color.Black) }, new Point(0, 0), Color.Black),
                new FigureState(new Rook{ Image = imagesInitializer?.GetImage<Rook>(Color.Black) }, new Point(7, 0), Color.Black),
                new FigureState(new Rook{ Image = imagesInitializer?.GetImage<Rook>(Color.White) }, new Point(0, 7), Color.White),
                new FigureState(new Rook{ Image = imagesInitializer?.GetImage<Rook>(Color.White) }, new Point(7, 7), Color.White),
                new FigureState(new Knight{ Image = imagesInitializer?.GetImage<Knight>(Color.Black) }, new Point(1, 0), Color.Black),
                new FigureState(new Knight{ Image = imagesInitializer?.GetImage<Knight>(Color.Black) }, new Point(6, 0), Color.Black),
                new FigureState(new Knight{ Image = imagesInitializer?.GetImage<Knight>(Color.White) }, new Point(1, 7), Color.White),
                new FigureState(new Knight{ Image = imagesInitializer?.GetImage<Knight>(Color.White) }, new Point(6, 7), Color.White),
                new FigureState(new Bishop{ Image = imagesInitializer?.GetImage<Bishop>(Color.Black) }, new Point(2, 0), Color.Black),
                new FigureState(new Bishop{ Image = imagesInitializer?.GetImage<Bishop>(Color.Black) }, new Point(5, 0), Color.Black),
                new FigureState(new Bishop{ Image = imagesInitializer?.GetImage<Bishop>(Color.White) }, new Point(2, 7), Color.White),
                new FigureState(new Bishop{ Image = imagesInitializer?.GetImage<Bishop>(Color.White) }, new Point(5, 7), Color.White),
                new FigureState(new Queen{ Image = imagesInitializer?.GetImage<Queen>(Color.Black) }, new Point(3, 0), Color.Black),
                new FigureState(new Queen{ Image = imagesInitializer?.GetImage<Queen>(Color.White) }, new Point(3, 7), Color.White),
                new FigureState(new King{ Image = imagesInitializer?.GetImage<King>(Color.Black) }, new Point(4, 0), Color.Black),
                new FigureState(new King{ Image = imagesInitializer?.GetImage<King>(Color.White) }, new Point(4, 7), Color.White)
            };
            for (int i = 0; i < BOARD_WIDTH; i++)
            {
                figures.Add(new FigureState(new Pawn(false) { Image = imagesInitializer?.GetImage<Pawn>(Color.Black) }, new Point(i, 1), Color.Black));
                figures.Add(new FigureState(new Pawn(true) { Image = imagesInitializer?.GetImage<Pawn>(Color.White) }, new Point(i, 6), Color.White));
            }

            return figures;
        }

        public FigureState GetFigure(Figure figure, Point position, Color color)
        {
            return new FigureState(figure, position, color);
        }
    }
}