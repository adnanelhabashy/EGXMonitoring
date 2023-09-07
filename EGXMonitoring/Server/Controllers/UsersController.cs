using EGXMonitoring.Shared;
using EGXMonitoring.Shared.DTOS;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EGXMonitoring.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService)
        {
                _userService = userService;
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<User>>>> GetUsers()
        {
            var response = await _userService.GetUsers();
            return Ok(response);
        }

        [HttpPost("adduser")]
        public async Task<ActionResult<ServiceResponse<User>>> AddUser(User user)
        {
            var response = await _userService.AddUser(user);
            return Ok(response);
        }

        [HttpPost("updateuser")]
        public async Task<ActionResult<ServiceResponse<User>>> UpdateUser(User user)
        {
            var response = await _userService.UpdateUser(user);
            return Ok(response);
        }

        [HttpPost("deleteuser")]
        public async Task<ActionResult<ServiceResponse<User>>> DeleteUser(User user)
        {
            var response = await _userService.DeleteUser(user);
            return Ok(response);
        }


    }
}
