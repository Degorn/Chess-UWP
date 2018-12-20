using Microsoft.EntityFrameworkCore;

namespace Chess_UWP.Database
{
    public class ChessDbContext : DbContext
    {
        public DbSet<GameInfo> GameInfos { get; set; }

        public ChessDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = Chess.db");
        }
    }
}
