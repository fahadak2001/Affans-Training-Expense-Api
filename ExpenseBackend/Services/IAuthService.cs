namespace ExpenseBackend.Services
{
    public interface IAuthService
    {
        bool Authenticate(string username);
    }
}
