using System.Net;
using Business.Abstract;
using Core.Entities.Concrete;
using Core.Utilities.Results.Abstract;
using Core.Utilities.Results.Concrete.ErrorResults;
using Core.Utilities.Results.Concrete.SuccessResults;
using Entities.DTOs.RoleDTOs;
using Microsoft.AspNetCore.Identity;

namespace Business.Concrete;

public class RoleManager(RoleManager<AppRole> roleManager) : IRoleService
{
    public async Task<IResult> CreateRoleAsync(string roleName)
    {
        IdentityResult identityResult = await roleManager.CreateAsync(new()
        {
            Id = Guid.NewGuid().ToString(),
            Name = roleName
        });

        if (identityResult.Succeeded)
            return new SuccessResult(HttpStatusCode.OK);
        else
        {
            string responseMessage = string.Empty;
            foreach (var error in identityResult.Errors)
                responseMessage += $"{error.Description}. ";
            return new ErrorResult(message: responseMessage, HttpStatusCode.BadRequest);
        }
    }

    public async Task<IResult> DeleteRoleAsync(string id)
    {
        AppRole? appRole = await roleManager.FindByIdAsync(id);
        if (appRole == null)
            return new ErrorResult(statusCode: HttpStatusCode.NotFound);

        IdentityResult identityResult = await roleManager.DeleteAsync(appRole);
        if (identityResult.Succeeded)
            return new SuccessResult(HttpStatusCode.OK);
        else
        {
            string responseMessage = string.Empty;
            foreach (var error in identityResult.Errors)
                responseMessage += $"{error.Description}. ";
            return new ErrorResult(message: responseMessage, HttpStatusCode.BadRequest);
        }
    }

    public async Task<IResult> UpdateRoleAsync(string roleId, string roleName)
    {
        AppRole? appRole = await roleManager.FindByIdAsync(roleId);
        if (appRole == null)
            return new ErrorResult(statusCode: HttpStatusCode.NotFound);
        appRole.Name = roleName;
        IdentityResult identityResult = await roleManager.UpdateAsync(appRole);
        if (identityResult.Succeeded)
            return new SuccessResult(HttpStatusCode.OK);
        else
        {
            string responseMessage = string.Empty;
            foreach (var error in identityResult.Errors)
                responseMessage += $"{error.Description}. ";
            return new ErrorResult(message: responseMessage, HttpStatusCode.BadRequest);
        }
    }

    public IDataResult<List<GetRoleDto>> GetAllRoles()
    {
        var result = roleManager.Roles.ToList();
        var response = result.Select(x => new GetRoleDto(x.Id, x.Name)).ToList();
        return new SuccessDataResult<List<GetRoleDto>>(data: response, statusCode: HttpStatusCode.OK);
    }
}