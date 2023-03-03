using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OberMind.PurchaseOrders.Application.DTOs;
using OberMind.PurchaseOrders.Application.Services;

namespace OberMind.PurchaseOrders.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;   
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO user)
        {
            var token = _userService.Login(user);
            if (token is null) { 
                return NotFound(new {error = "User not found"});
            }
            return Ok(new {token = token});
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO user)
        {
            try
            {
                await _userService.Register(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new {error= ex.Message});
            }
        }

    }
}
