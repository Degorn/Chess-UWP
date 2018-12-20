using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chess_UWP.Database
{
    public class Repository : IRepository
    {
        private static readonly Lazy<Repository> repository = new Lazy<Repository>(() => new Repository(IoC.Get<ChessDbContext>()));
        private readonly ChessDbContext dbContext;

        public static Repository Instance => repository.Value;

        public Repository(ChessDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task AddAsync(GameInfo gameInfo)
        {
            using (ChessDbContext db = new ChessDbContext())
            {
                await db.GameInfos.AddAsync(gameInfo);
                await db.SaveChangesAsync();
            }
        }

        public IEnumerable<GameInfo> GetAll()
        {
            using (ChessDbContext db = new ChessDbContext())
            {
                return db.GameInfos.ToList();
            }
        }

        public async Task ClearGameStatisticsAsync()
        {
            using (ChessDbContext db = new ChessDbContext())
            {
                db.GameInfos.RemoveRange(db.GameInfos);
                await db.SaveChangesAsync();
            }
        }
    }
}
