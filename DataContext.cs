using Microsoft.EntityFrameworkCore;
using misha_kris_finance_bot.Models;

namespace misha_kris_finance_bot
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL("server=127.0.0.1;database=financedb;user=root;password=admin");
        }
    }
}
