using System.Collections.Generic;
using Chess_UWP.Models;

namespace Chess_UWP.Infrastructure.Initializers
{
    public interface ICellsInitializer
    {
        IEnumerable<BoardCell> GetBoardCells(int boardWidth, int boardHeight);
    }
}