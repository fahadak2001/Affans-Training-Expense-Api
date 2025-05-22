using ExpenseBackend.Data;
using ExpenseBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ExpenseBackend.Repositories
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseBackendDBContext _dbContext;

        public ExpenseRepository(ExpenseBackendDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Expense GetById(int id)
        {
            var expense = _dbContext.Expense.Find(id);
            if (expense == null)
            {
                throw new KeyNotFoundException($"Expense with ID {id} not found.");
            }
            return expense;
        }

        public IEnumerable<Expense> GetAll(string userName)
        {
            // Here I am using a stored procedure to get the expenses by username.
            // The username that I retrieved from the JWT token.
            return _dbContext.Expense.FromSqlRaw("EXEC GetExpensesByUsername @UserName={0}", userName).ToList();
        }
  
        public void Create(Expense expense)
        {
            _dbContext.Expense.Add(expense);
            _dbContext.SaveChanges();
        }

        public void Update(Expense expense)
        {
            _dbContext.Expense.Update(expense);
            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            var expenseToDelete = _dbContext.Expense.Find(id);
            if (expenseToDelete != null)
            {
                _dbContext.Expense.Remove(expenseToDelete);
                _dbContext.SaveChanges();
            }
        }
    }
}
