using Bank_Web_Api.Model;
using Microsoft.EntityFrameworkCore;

namespace Bank_Web_Api.Data
{
    public class BankingDbContext:DbContext
    {
        public BankingDbContext(DbContextOptions<BankingDbContext> options):base(options)
        {

        }
        //dbsets
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
