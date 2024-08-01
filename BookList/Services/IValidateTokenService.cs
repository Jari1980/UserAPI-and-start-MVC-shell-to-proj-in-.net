using BookList.Models.ViewModells;

namespace BookList.Services
{
    public interface IValidateTokenService
    {
        bool ValidateToken(SessionVM sessionObject);
    }
}