using BookList.Models.ViewModells;
using System.IdentityModel.Tokens.Jwt;

namespace BookList.Services
{
    public class ValidateTokenService : IValidateTokenService
    {
        const string SessionKeyToken = "SessionKey";
        
        public bool ValidateToken(SessionVM sessionObject)
        {
            

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var token = handler.ReadToken(sessionObject.token) as JwtSecurityToken;

                if (token.ValidTo > DateTime.Now && token.ValidTo < DateTime.Now.AddDays(1))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
