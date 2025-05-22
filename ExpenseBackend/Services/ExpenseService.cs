using ExpenseBackend.Models;
using ExpenseBackend.Repositories;

namespace ExpenseBackend.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        public ExpenseService(IExpenseRepository expenseRepository)
        {
            _expenseRepository = expenseRepository;
        }
        public Expense GetExpenseById(int id)
        {
            return _expenseRepository.GetById(id);
        }
        public IEnumerable<Expense> GetAllExpenses(string userName)
        {
            return _expenseRepository.GetAll(userName);
        }
        public void CreateExpense(Expense expense)
        {
            _expenseRepository.Create(expense);
        }
        public void UpdateExpense(Expense expense)
        {
            _expenseRepository.Update(expense);
        }
        public void DeleteExpense(int id)
        {
            _expenseRepository.Delete(id);
        }
    }
}
