using System.Collections.Generic;

namespace Chess_UWP.Database
{
    public interface IRepository
    {
        void AddAsync(GameInfo gameInfo);
        IEnumerable<GameInfo> GetAll();
        void ClearGameStatisticsAsync();
    }
}