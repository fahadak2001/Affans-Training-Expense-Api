using ExpenseBackend.Models;

namespace ExpenseBackend.Repositories
{
    public interface IExpenseRepository
    {
        Expense GetById(int id);
        IEnumerable<Expense> GetAll(string userName);
        void Create(Expense expense);
        void Update(Expense expense);
        void Delete(int id);
    }
}
