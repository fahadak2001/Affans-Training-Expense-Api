using ExpenseBackend.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using System.Text.Json;


namespace ExpenseBackend.Services
{
    public class CachedExpenseService : ICachedExpenseService
    {
        private readonly IExpenseService _decoratedExpenseService;
        private readonly IDistributedCache _distributedCache;

        public CachedExpenseService(IExpenseService decoratedExpenseService, IDistributedCache distributedCache)
        {
            _decoratedExpenseService = decoratedExpenseService;
            _distributedCache = distributedCache;
        }

        public Expense GetExpenseById(int id)
        {
            return _decoratedExpenseService.GetExpenseById(id);
        }

        public IEnumerable<Expense> GetAllExpenses(string userName)
        {
            string cacheKey = $"expenses:{userName}";
            byte[]? cachedExpensesBytes = _distributedCache.Get(cacheKey);

            if (cachedExpensesBytes != null)
            {
                var cachedExpenses = JsonSerializer.Deserialize<List<Expense>>(Encoding.UTF8.GetString(cachedExpensesBytes));
                return cachedExpenses;
            }

            IEnumerable<Expense> expenses = _decoratedExpenseService.GetAllExpenses(userName);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(5));

            var expensesJson = JsonSerializer.Serialize(expenses.ToList());
            var expensesBytes = Encoding.UTF8.GetBytes(expensesJson);
            _distributedCache.Set(cacheKey, expensesBytes, options);

            return expenses;
        }

        public void CreateExpense(Expense expense)
        {
            _decoratedExpenseService.CreateExpense(expense);
            
            string cacheKey = $"expenses:{expense.UserName}";
            _distributedCache.Remove(cacheKey);
        }

        public void UpdateExpense(Expense expense)
        {
            _decoratedExpenseService.UpdateExpense(expense);
            
            string cacheKey = $"expenses:{expense.UserName}";
            _distributedCache.Remove(cacheKey);
        }

        public void DeleteExpense(int id)
        {
            var expenseToDelete = _decoratedExpenseService.GetExpenseById(id);
            if (expenseToDelete != null)
            {
                _decoratedExpenseService.DeleteExpense(id);
                string cacheKey = $"expenses:{expenseToDelete.UserName}";
                _distributedCache.Remove(cacheKey);
            }
        }
    }
}