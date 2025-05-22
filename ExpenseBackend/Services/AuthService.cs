namespace ExpenseBackend.Services
{
    public class AuthService : IAuthService
    {
        public bool Authenticate(string username)
        {
            return !string.IsNullOrEmpty(username);
        }
    }
}
