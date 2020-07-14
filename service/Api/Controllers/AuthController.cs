using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Interface;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseHelperController
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;

        public AuthController(IUserService userService, ILogger<AuthController> logger,
            IConfiguration configuration)
        {
            _userService = userService;
            _logger = logger;
            _configuration = configuration;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto model)
        {
            if (ModelState.IsValid)
            {
                var confirmationLink = _configuration["HelperUrls:FrontendUrl"] + "verifyaccount";
                var result = await _userService.RegisterUserAsync(model, confirmationLink);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            SetModelStateErrorMessage();
            return BadRequest(response);
        }

        [AllowAnonymous]
        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmailAsync(string userId, string token)
        {
            if (userId == null || token == null)
            {
                SetErrorMessage();
                return BadRequest(response);
            }

            var result = await _userService.ConfirmEmailAsync(userId, token);
            if (result.IsSuccess)
                return Ok(result);
            return BadRequest(result);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginDto model)
        {
            if (ModelState.IsValid)
            {
                var result = await _userService.LoginUserAsync(model);

                if (result.IsSuccess)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }

            SetModelStateErrorMessage();
            return BadRequest(response);
        }

        [HttpPost("ForgetPassword")]
        public async Task<IActionResult> ForgetPasswordAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return NotFound();
            }

            var confirmationLink = Url.ActionLink("ResetPasswordAsync", "Auth");
            var result = await _userService.ForgotPassword(email, confirmationLink);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync(ForgetPasswordDto model)
        {
            if (!ModelState.IsValid)
            {
                SetErrorMessage();
                return BadRequest(response);
            }

            var result = await _userService.ResetPassword(model);

            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

    }
}
