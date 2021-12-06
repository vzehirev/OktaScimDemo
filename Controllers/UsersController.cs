using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Okta.Auth.Sdk;
using OktaScimDemo.Models;
using OktaScimDemo.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace OktaScimDemo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UsersService usersService;

        public UsersController(UsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("First name, last name, e-mail and password are requried!");
            }
            var result = await usersService.RegisterUser(model);

            return StatusCode(201, result);
        }

        [HttpPost]
        [Route("logIn")]
        public async Task<IActionResult> LogInUser(LogInRequest model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception("E-mail and password are requried!");
            }

            var result = await usersService.AuthenticateUserAsync(model);
            if (result.AuthenticationStatus != AuthenticationStatus.Success)
            {
                throw new Exception("Wrong credentials!");
            }

            var userId = await usersService.GetUserIdAsync(model.Email);

            var token = usersService.GenerateJwt(userId);
            return Ok(new { token });
        }

        [HttpGet]
        [Authorize]
        [Route("{userId}")]
        public async Task<IActionResult> GetUser(string userId)
        {
            var userIdFromToken = User.Claims.Single(x => x.Type == "userId").Value;
            if (userId != userIdFromToken)
            {
                throw new Exception("Not allowed to update user!");
            }

            var result = await usersService.GetUserAsync(userId);
            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [Route("{userId}")]
        public async Task<IActionResult> UpdateUser(string userId, UpdateUserRequest model)
        {
            var userIdFromToken = User.Claims.Single(x => x.Type == "userId").Value;
            if (userId != userIdFromToken)
            {
                throw new Exception("Not allowed to update user!");
            }

            var result = await usersService.UpdateUserAsync(userId, model);
            return Ok(result);
        }
    }
}
