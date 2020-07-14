using BusinessLogic.Constants;
using BusinessLogic.Interface;
using Data.DBTables;
using DataTransferObject.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Service
{
    public class UserService : BaseService, IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IMailService _mailService;

        public UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IMailService mailService, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
            _logger = logger;
        }

        public async Task<ResponseDto> RegisterUserAsync(RegisterDto model, string confirmationLink)
        {
            var identityUser = new ApplicationUser
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(identityUser, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                token = System.Web.HttpUtility.UrlEncode(token);
                confirmationLink += string.Format(MessageConstant.User.RegistrationLink, identityUser.Id, token);
                string message = string.Format(MessageConstant.User.RegistrationMailTemplate, DateTime.Now, confirmationLink);
                var isMailSent = await _mailService.SendMailAsync(model.Email, message, MessageConstant.User.RegistrationMailSubject);

                response.Message = MessageConstant.User.UserCreated;
                return response;
            }
            response.IsSuccess = false;
            response.Message = MessageConstant.User.UserNotCreated; ;
            response.Errors = result.Errors.Select(e => e.Description);

            return response;
        }

        private JwtSecurityToken GenerateToken(string email, string id)
        {
            var claims = new[] {
                new Claim (MessageConstant.User.Claims.Email, email),
                new Claim (ClaimTypes.NameIdentifier, id)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration[ConfigurationConstant.User.JwtKey]));

            var token = new JwtSecurityToken(
                issuer: _configuration[ConfigurationConstant.User.JwtIssuer],
                audience: _configuration[ConfigurationConstant.User.JwtAudience],
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

            return token;
        }

        public async Task<ResponseDto> LoginUserAsync(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                response.Message = MessageConstant.User.UserNotFound;
                response.IsSuccess = false;
                return response;
            }
            var isAccountVerified = await _userManager.IsEmailConfirmedAsync(user);
            if (!isAccountVerified)
            {
                response.IsSuccess = false;
                response.Message = MessageConstant.User.UserNotVerified;
                return response;
            }
            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (!result)
            {
                response.IsSuccess = false;
                response.Message = MessageConstant.User.IncorrectPassword;
                return response;
            }


            var token = GenerateToken(model.Email, user.Id);
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            string message = string.Format(MessageConstant.User.NewLoginNotification, DateTime.Now);

            var isMailSent = await _mailService.SendMailAsync(model.Email, message, MessageConstant.User.NewLogindDetected);
            response.Message = MessageConstant.User.SuccessfulLogin;
            response.Data = new
            {
                ExpireDate = token.ValidTo,
                Token = tokenString,
                user.FirstName,
                user.LastName
            };
            return response;
        }

        public async Task<ResponseDto> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = MessageConstant.User.UserNotFoundById;
                return response;
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                var jwtToken = GenerateToken(user.Email, user.Id);
                string tokenString = new JwtSecurityTokenHandler().WriteToken(jwtToken);

                response.Data = new
                {
                    ExpireDate = jwtToken.ValidTo,
                    Token = tokenString,
                    user.FirstName,
                    user.LastName
                };
                response.Message = MessageConstant.User.AccountActivated;
                return response;
            }
            response.Message = MessageConstant.User.AccountActivationFailed;
            response.IsSuccess = false;
            return response;
        }

        public async Task<ResponseDto> ForgotPassword(string email, string confirmationLink)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                response.IsSuccess = false;
                response.Message = MessageConstant.User.UserNotFound;
                return response;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodeToken = Encoding.UTF8.GetBytes(token);
            var validEmailToken = WebEncoders.Base64UrlEncode(encodeToken);

            confirmationLink += string.Format(MessageConstant.User.ConfirmationLink, email, token);
            string message = string.Format(MessageConstant.User.ResetPasswordTemplate, confirmationLink);
            var isMailSent = await _mailService.SendMailAsync(email, message, MessageConstant.User.PasswordResetSubject);
            response.Message = MessageConstant.User.PasswordLinkSend;
            return response;

        }

        public async Task<ResponseDto> ResetPassword(ForgetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                response.Message = MessageConstant.User.UserNotFound;
                response.IsSuccess = false;
                return response;
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            model.Token = Encoding.UTF8.GetString(decodedToken);

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded)
            {
                response.Message = MessageConstant.User.PasswordChanged;

                return response;
            }
            response.Message = MessageConstant.User.PasswordChangedFailed;
            response.IsSuccess = true;
            return response;
        }
    }
}
