using Core.Entities.Concrete;

namespace Core.Utilities.Security.Abstract
{
    public interface ITokenService
    {
        Task<Token> CreateAccessTokenAsync(AppUser appUser, List<string> roles);
    }
}
