using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BookYourShow.Data.EFDatabase;
using BookYourShow.DataTransferObject.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace BookYourShow.BusinessLogic.Service {
    public interface IUserService {
        Task<UserManagerResponseDto> RegisterUserAsync (RegisterDto model, string confirmationLink);
        Task<UserManagerResponseDto> LoginUserAsync (LoginDto model);
        Task<UserManagerResponseDto> ConfirmEmailAsync (string userId, string token);
        Task<UserManagerResponseDto> ForgotPassword (string email, string confirmationLink);
        Task<UserManagerResponseDto> ResetPassword(ForgetPasswordDto model);
    }
    public class UserService : IUserService {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserService> _logger;
        private readonly IMailService _mailService;

        public UserService (UserManager<ApplicationUser> userManager, IConfiguration configuration,
            IMailService mailService, ILogger<UserService> logger) {
            _userManager = userManager;
            _configuration = configuration;
            _mailService = mailService;
             _logger = logger;
        }

        public async Task<UserManagerResponseDto> RegisterUserAsync (RegisterDto model, string confirmationLink) {
            var identityUser = new ApplicationUser {
                Email = model.Email,
                FullName = model.FullName,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync (identityUser, model.Password);

            if (result.Succeeded) {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync (identityUser);
                token = System.Web.HttpUtility.UrlEncode (token);
                confirmationLink += $"?userId={identityUser.Id}&token={token}";
                string message = $"<p>New account have been created at {DateTime.Now}." +
                    $"kindly use the below link to activate your account <br /> <br> <a href='{confirmationLink}'> Verify account </a></p>";

                var isMailSent = await _mailService.SendMailAsync (model.Email, message, "Activate your account");

                return new UserManagerResponseDto {
                    Message = "User Created successfully",
                        IsSuccess = true
                };
            }
            return new UserManagerResponseDto {
                Message = "User did not created",
                    IsSuccess = false,
                    Errors = result.Errors.Select (e => e.Description)
            };
        }

        private JwtSecurityToken GenerateToken(string email, string id){
             var claims = new [] {
                new Claim ("Email", email),
                new Claim (ClaimTypes.NameIdentifier, id)
            };

            var key = new SymmetricSecurityKey (
                Encoding.UTF8.GetBytes (_configuration["AuthSettings:Key"]));

            var token = new JwtSecurityToken (
                issuer: _configuration["AuthSettings:Audience"],
                audience : _configuration["AuthSettings:Issuer"],
                claims : claims,
                expires : DateTime.Now.AddDays (10),
                signingCredentials : new SigningCredentials (key, SecurityAlgorithms.HmacSha256));

            return token;
        }

        public async Task<UserManagerResponseDto> LoginUserAsync (LoginDto model) {
            var user = await _userManager.FindByEmailAsync (model.Email);

            if (user == null) {
                return new UserManagerResponseDto {
                Message = "There is no user with the provided Email Address",
                IsSuccess = false
                };
            }
            var isAccountVerified = await _userManager.IsEmailConfirmedAsync (user);
            if (!isAccountVerified) {
                return new UserManagerResponseDto {
                    Message = "User Account not verified please verify it first.....",
                    IsSuccess = false
                };
            }
            var result = await _userManager.CheckPasswordAsync (user, model.Password);

            if (!result) {
                return new UserManagerResponseDto {
                    Message = "User Password incorrect",
                    IsSuccess = false
                };
            }

           
            var token = GenerateToken(model.Email, user.Id);
            string tokenString = new JwtSecurityTokenHandler ().WriteToken (token);
            string message = $"New login to your account have been noticed at {DateTime.Now}";

            var isMailSent = await _mailService.SendMailAsync (model.Email, message, "Login successful");

            return new UserManagerResponseDto {
                Message = "Sucessfuly logged in",
                    IsSuccess = true,
                    Data = new {
                        ExpireDate = token.ValidTo,
                        Token = tokenString,
                        FullName = user.FullName
                    }
            };
        }

        public async Task<UserManagerResponseDto> ConfirmEmailAsync (string userId, string token) {
            var user = await _userManager.FindByIdAsync (userId);
            if (user == null) {
                return new UserManagerResponseDto {
                Message = "There is no user with the provided User Id",
                IsSuccess = false
                };
            }

            var result = await _userManager.ConfirmEmailAsync (user, token);
            if (result.Succeeded) {
                var jwtToken = GenerateToken(user.Email, user.Id);
                string tokenString = new JwtSecurityTokenHandler ().WriteToken (jwtToken);

                return new UserManagerResponseDto {
                    Message = "Account successfuly activated",
                    IsSuccess = true,
                    Data = new {
                        ExpireDate = jwtToken.ValidTo,
                        Token = tokenString,
                        FullName = user.FullName
                    }
                };
            }
            return new UserManagerResponseDto {
                Message = "Account activation failed",
                IsSuccess = false
            };
        }

        public async Task<UserManagerResponseDto> ForgotPassword (string email, string confirmationLink) {
            var user = await _userManager.FindByEmailAsync (email);

            if (user == null) {
                return new UserManagerResponseDto {
                Message = "There is no user with the provided Email Address",
                IsSuccess = false
                };
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync (user);
            var encodeToken = Encoding.UTF8.GetBytes (token);
            var validEmailToken = WebEncoders.Base64UrlEncode (encodeToken);

            confirmationLink += $"?Email={email}&token={token}";
            string message = $"<p>Click on the below link to reset your password." +
                $" <br /> <br> <a href='{confirmationLink}'> Reset Password  </a></p>";

            var isMailSent = await _mailService.SendMailAsync (email, message, "Password Reset");

            return new UserManagerResponseDto {
                Message = "Password change link send",
                    IsSuccess = true
            };

        }

        public async Task<UserManagerResponseDto> ResetPassword(ForgetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync (model.Email);

             if (user == null) {
                return new UserManagerResponseDto {
                Message = "There is no user with the provided Email Address",
                IsSuccess = false
                };
            }

            var decodedToken = WebEncoders.Base64UrlDecode(model.Token);
            model.Token = Encoding.UTF8.GetString(decodedToken);
            
            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);

            if (result.Succeeded) {
                return new UserManagerResponseDto {
                    Message = "Password successfully changed",
                        IsSuccess = true
                };
            }
            return new UserManagerResponseDto {
                Message = "Password change failed",
                    IsSuccess = true
            };
        }
    }
}