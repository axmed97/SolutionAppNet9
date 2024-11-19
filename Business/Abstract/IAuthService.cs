using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Entities.DTOs.AuthDTOs;

namespace Business.Abstract
{
    public interface IAuthService
    {
        Task<IResult> RegisterAsync(RegisterDto model);
        Task<IDataResult<string>> UpdateRefreshTokenAsync(string refreshToken, AppUser user);
        Task<IDataResult<Token>> LoginAsync(LoginDto model);
        Task<IDataResult<Token>> RefreshTokenLoginAsync(RefreshTokenDto model);
        Task<IResult> LogOutAsync(string userId);
        Task<IDataResult<List<UserDto>>> GetAllAsync();
        Task<IResult> RemoveUserAsync(string userId);
        Task<IDataResult<Token>> GoogleAuthAsync(GoogleAuthDto model);

    }
}
