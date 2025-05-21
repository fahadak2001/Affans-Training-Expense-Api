using Microsoft.EntityFrameworkCore;
using ExpenseBackend.Models;

namespace ExpenseBackend.Data
{
    public class ExpenseBackendDBContext : DbContext
    {
        public DbSet<Expense> Expense { get; set; }

        public ExpenseBackendDBContext(DbContextOptions<ExpenseBackendDBContext> options) : base(options)
        {
        }
    }
}