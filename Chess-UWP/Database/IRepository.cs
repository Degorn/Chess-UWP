using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chess_UWP.Database
{
    public interface IRepository
    {
        Task AddAsync(GameInfo gameInfo);
        IEnumerable<GameInfo> GetAll();
        Task ClearGameStatisticsAsync();
    }
}