using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLogic.Constants
{
    public static class ConfigurationConstant
    {
        public struct User
        {
            public const string JwtKey = "AuthSettings:Key";
            public const string JwtAudience = "AuthSettings:Audience";
            public const string JwtIssuer = "AuthSettings:Issuer";
        }
        public struct Mail
        {
            public const string FromEmail = "MailSettings:FromEmail";
            public const string Name = "MailSettings:FromEmail";
            public const string Password = "MailSettings:FromPassword";
            public const string SmtpHost = "MailSettings:FromPassword";
            public const int SmtpPort = 587;
            public const int SmtpTimeOut = 20000;
        }
    }
}
