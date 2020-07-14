using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Constants
{
    public static class MessageConstant
    {
        public struct Global
        {
            public const string InValidData = "Data is not valid";
        }
        public struct User
        {
            public struct Claims
            {
                public const string Email = "Email";
            }
            public const string RegistrationLink = "?userId={0}&token={1}";
            public const string RegistrationMailSubject = "Activate your account Fixed Ratio account";
            public const string RegistrationMailTemplate = "<p>New account have been created at {0}. kindly use the below link to activate your account <br /> <br> <a href='{1}'> Verify account </a></p>";
            public const string UserCreated = "User Created successfully";
            public const string UserNotCreated = "Failed to create user";
            public const string UserNotFound = "There is no user with the provided Email Address";
            public const string UserNotFoundById = "There is no user with the provided User Id";
            public const string UserNotVerified = "User Account not verified please verify it first.....";
            public const string IncorrectPassword = "User Password incorrect";
            public const string NewLoginNotification = "New login to your account have been noticed at {0}";
            public const string NewLogindDetected = "New Login Detected";
            public const string SuccessfulLogin = "Sucessfuly logged in";
            public const string AccountActivated = "Account successfuly activated";
            public const string AccountActivationFailed = "Account activation failed";
            public const string ConfirmationLink = "?Email={0}&token={1}";
            public const string ResetPasswordTemplate = "<p>Click on the below link to reset your password.<br /> <br> <a href='{0}'> Reset Password  </a></p>";
            public const string PasswordResetSubject = "Password Reset";
            public const string PasswordLinkSend = "Password change link send";
            public const string PasswordChanged = "Password successfully changed";
            public const string PasswordChangedFailed = "Failed to change password";
        }
    }
}
