using DataTransferObject.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IUserService
    {
        Task<ResponseDto> RegisterUserAsync(RegisterDto model, string confirmationLink);
        Task<ResponseDto> LoginUserAsync(LoginDto model);
        Task<ResponseDto> ConfirmEmailAsync(string userId, string token);
        Task<ResponseDto> ForgotPassword(string email, string confirmationLink);
        Task<ResponseDto> ResetPassword(ForgetPasswordDto model);
    }
}
