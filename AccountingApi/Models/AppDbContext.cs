using Microsoft.EntityFrameworkCore;

namespace AccountingApi.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 這行代表資料庫裡會有一張叫 Transactions 的資料表
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
