using Business.Abstract;
using Business.StatusMessages;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using Core.Utilities.Security.Abstract;
using Entities.Concrete;
using Entities.DTOs.AuthDTOs;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Business.Concrete
{
    public class AuthManager(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,
        ITokenService tokenService,
        IConfiguration configuration)
        : IAuthService
    {
        public async Task<IResult> RegisterAsync(RegisterDto model)
        {
            AppUser? checkEmail = await userManager.Users.FirstOrDefaultAsync(x => x.Email == model.Email);
            if (checkEmail != null)
                return new ErrorResult(message: Message.EmailAlreadyExists, statusCode: HttpStatusCode.BadRequest);

            AppUser? checkUserName = await userManager.FindByNameAsync(model.UserName);
            if (checkUserName != null)
                return new ErrorResult(message: Message.UsernameAlreadyExists, statusCode: HttpStatusCode.BadRequest);

            User newUser = new()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.UserName,
            };

            IdentityResult identityResult = await userManager.CreateAsync(newUser, model.Password);

            if (identityResult.Succeeded)
                return new SuccessResult(message: Message.RegistrationSuccess, statusCode: HttpStatusCode.Created);
            else
            {
                string responseMessage = string.Empty;
                foreach (IdentityError error in identityResult.Errors)
                    responseMessage += $"{error.Description}. ";
                return new ErrorResult(message: responseMessage, HttpStatusCode.BadRequest);
            }
        }

        public async Task<IDataResult<string>> UpdateRefreshTokenAsync(string refreshToken, AppUser user)
        {
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiredDate = DateTime.UtcNow.AddMonths(1);

                IdentityResult identityResult = await userManager.UpdateAsync(user);

                if (identityResult.Succeeded)
                    return new SuccessDataResult<string>(statusCode: HttpStatusCode.OK, data: refreshToken);
                else
                {
                    string responseMessage = string.Empty;
                    foreach (IdentityError error in identityResult.Errors)
                        responseMessage += $"{error.Description}. ";
                    return new ErrorDataResult<string>(message: responseMessage, HttpStatusCode.BadRequest);
                }
            }
            else
                return new ErrorDataResult<string>(Message.UserNotFound, HttpStatusCode.NotFound);
        }

        public async Task<IDataResult<Token>> LoginAsync(LoginDto model)
        {
            AppUser? user = await userManager.FindByNameAsync(model.EmailOrUsername);
            user ??= await userManager.FindByEmailAsync(model.EmailOrUsername);

            if (user == null)
                return new ErrorDataResult<Token>(Message.UserNotFound, HttpStatusCode.NotFound);

            SignInResult result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            IList<string> roles = await userManager.GetRolesAsync(user);
            if (result.Succeeded)
            {
                Token token = await tokenService.CreateAccessTokenAsync(user, roles.ToList());
                IDataResult<string> response = await UpdateRefreshTokenAsync(refreshToken: token.RefreshToken, user);
                if (response.Success)
                    return new SuccessDataResult<Token>(data: token, statusCode: HttpStatusCode.OK,
                        message: response.Message);
                else
                    return new ErrorDataResult<Token>(statusCode: HttpStatusCode.BadRequest, message: response.Message);
            }
            else
                return new ErrorDataResult<Token>(statusCode: HttpStatusCode.BadRequest, message: Message.UserNotFound);
        }

        public async Task<IDataResult<Token>> RefreshTokenLoginAsync(RefreshTokenDto model)
        {
            AppUser? user = await userManager.Users.FirstOrDefaultAsync(x => x.RefreshToken == model.RefreshToken);
            if (user is not null)
            {
                IList<string> roles = await userManager.GetRolesAsync(user);
                if (user.RefreshTokenExpiredDate > DateTime.UtcNow.AddHours(4))
                {
                    Token token = await tokenService.CreateAccessTokenAsync(user, roles.ToList());
                    token.RefreshToken = model.RefreshToken;
                    return new SuccessDataResult<Token>(data: token, statusCode: HttpStatusCode.OK);
                }
            }

            return new ErrorDataResult<Token>(statusCode: HttpStatusCode.BadRequest, message: Message.UserNotFound);
        }

        public async Task<IResult> LogOutAsync(string userId)
        {
            AppUser? user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new ErrorResult(statusCode: HttpStatusCode.NotFound, message: Message.UserNotFound);

            user.RefreshToken = null;
            user.RefreshTokenExpiredDate = null;
            IdentityResult result = await userManager.UpdateAsync(user);

            if (result.Succeeded)
                return new SuccessResult(statusCode: HttpStatusCode.OK);
            else
            {
                string responseMessage = string.Empty;
                foreach (IdentityError error in result.Errors)
                {
                    responseMessage += error + ". ";
                }

                ;
                return new ErrorDataResult<Token>(statusCode: HttpStatusCode.BadRequest, message: responseMessage);
            }
        }

        public async Task<IDataResult<List<UserDto>>> GetAllAsync()
        {
            var result = await userManager.Users.OfType<AppUser>().ToListAsync();
            List<UserDto> users = result.Select(x => new UserDto
            {
                Id = x.Id,
                Email = x.Email,
                Username = x.UserName
            }).ToList();

            return new SuccessDataResult<List<UserDto>>(data: users, statusCode: HttpStatusCode.OK);
        }

        public async Task<IResult> RemoveUserAsync(string userId)
        {
            AppUser? user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return new ErrorResult(statusCode: HttpStatusCode.BadRequest, message: Message.UserNotFound);

            IdentityResult result = await userManager.DeleteAsync(user);

            if (result.Succeeded)
                return new SuccessResult(HttpStatusCode.OK);
            else
            {
                string response = string.Empty;
                foreach (IdentityError error in result.Errors)
                {
                    response += error.Description + ". ";
                }

                return new ErrorResult(message: response, HttpStatusCode.BadRequest);
            }
        }

        public async Task<IDataResult<Token>> GoogleAuthAsync(GoogleAuthDto model)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string?> { configuration["Google:ClientId"] }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(model.IdToken, settings);

            UserLoginInfo userLoginInfo = new(model.Provider, payload.Subject, model.Provider);
            var user = await userManager.FindByLoginAsync(userLoginInfo.LoginProvider, userLoginInfo.ProviderKey);

            bool result = user != null;
            if (user == null)
            {
                user = await userManager.FindByEmailAsync(payload.Email);
                if (user == null)
                {
                    user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Email = payload.Email,
                        UserName = payload.Email,
                        FirstName = payload.Name,
                        LastName = payload.Name,
                    };
                    var identityResult = await userManager.CreateAsync(user);
                    result = identityResult.Succeeded;
                }
            }

            if (result)
                await userManager.AddLoginAsync(user, userLoginInfo);
            else
                return new ErrorDataResult<Token>(statusCode: HttpStatusCode.BadRequest);

            var roles = await userManager.GetRolesAsync(user);
            Token token = await tokenService.CreateAccessTokenAsync(user, roles.ToList());
            // If you you have problem check it
            // var response = await UpdateRefreshTokenAsync(token.RefreshToken, user);
            return new SuccessDataResult<Token>(data: token, statusCode: HttpStatusCode.OK);
        }
    }
}