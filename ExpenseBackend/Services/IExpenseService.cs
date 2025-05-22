using ExpenseBackend.Models;

namespace ExpenseBackend.Services
{
    public interface IExpenseService
    {
        Expense GetExpenseById(int id);
        IEnumerable<Expense> GetAllExpenses(string userName);
        void CreateExpense(Expense expense);
        void UpdateExpense(Expense expense);
        void DeleteExpense(int id);
    }
}
