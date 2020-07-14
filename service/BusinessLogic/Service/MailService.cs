using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BusinessLogic.Constants;
using BusinessLogic.Interface;
using DataTransferObject.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BusinessLogic.Service
{
    class MailService : IMailService
    {
        private readonly ILogger<MailService> _logger;
        private readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration, ILogger<MailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> SendMailAsync(string toEmail, string bodyText,
            string subjectText, List<string> ccEmails = null)
        {
            var isMailSent = true;
            var senderEmail = _configuration[ConfigurationConstant.Mail.FromEmail];
            var senderName = _configuration[ConfigurationConstant.Mail.Name];
            var fromPassword = _configuration[ConfigurationConstant.Mail.Password];

            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(senderEmail);
                    mail.To.Add(toEmail);
                    mail.Subject = subjectText;
                    mail.Body = bodyText;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(ConfigurationConstant.Mail.SmtpHost, ConfigurationConstant.Mail.SmtpPort))
                    {
                        smtp.Credentials = new NetworkCredential(senderEmail, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = ConfigurationConstant.Mail.SmtpTimeOut;
                        await smtp.SendMailAsync(mail);
                    }
                }
            }
            catch (Exception ex)
            {
                isMailSent = false;
                _logger.LogError(ex, ex.Message);
            }

            return isMailSent;
        }
    }
}
