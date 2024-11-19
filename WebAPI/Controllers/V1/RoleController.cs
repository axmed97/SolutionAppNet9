using Asp.Versioning;
using Business.Abstract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{v:apiVersion}/[controller]")]
    [ApiController]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        [HttpPost]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] string roleName)
        {
            var result = await roleService.CreateRoleAsync(roleName);
            return StatusCode((int)result.StatusCode, result);
        }

        [HttpPut]
        [MapToApiVersion("1.0")]
        [Authorize]
        public async Task<IActionResult> UpdateRole([FromBody] string roleName, [FromRoute] string roleId)
        {
            var result = await roleService.UpdateRoleAsync(roleId, roleName);
            return StatusCode((int)result.StatusCode, result);
        }
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteRole([FromRoute] string id)
        {
            var response = await roleService.DeleteRoleAsync(id);
            return StatusCode((int)response.StatusCode, response);
        }
        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet]
        public IActionResult GetAllRoles()
        {
            var response = roleService.GetAllRoles();
            return StatusCode((int)response.StatusCode, response);
        }
    }
}
