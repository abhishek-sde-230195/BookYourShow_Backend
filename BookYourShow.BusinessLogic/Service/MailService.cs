using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BookYourShow.DataTransferObject.DTO;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BookYourShow.BusinessLogic.Service {
    public interface IMailService {
        Task<bool> SendMailAsync (string toEmail, string bodyText,
            string subjectText, List<string> ccEmails = null);
    }
    public class MailService : IMailService {
        private readonly ILogger<MailService> _logger;
        private readonly IConfiguration _configuration;
        public MailService (IConfiguration configuration, ILogger<MailService> logger) {
            _configuration = configuration;
            _logger = logger;
        }
        public async Task<bool> SendMailAsync (string toEmail, string bodyText,
            string subjectText, List<string> ccEmails = null) {
            var isMailSent = true;
            var senderEmail = _configuration["MailSettings:FromEmail"];
            var senderName = _configuration["MailSettings:Name"];
            var fromPassword = _configuration["MailSettings:FromPassword"];

            try {
                using (MailMessage mail = new MailMessage ()) {
                    mail.From = new MailAddress (senderEmail);
                    mail.To.Add (toEmail);
                    mail.Subject = subjectText;
                    mail.Body = bodyText;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient ("smtp.gmail.com", 587)) {
                        smtp.Credentials = new NetworkCredential (senderEmail, fromPassword);
                        smtp.EnableSsl = true;
                        smtp.Timeout = 20000;
                        await smtp.SendMailAsync (mail);
                    }
                }

            } catch (Exception ex) {
                isMailSent = false;
                _logger.LogError(ex, ex.Message);
            }

            return isMailSent;

            #region MailKit Defination

            // var messageToSend = new MimeMessage {
            //     Sender = new MailboxAddress (senderName, senderEmail),
            //     Subject = subjectText,
            // };

            // messageToSend.From.Add (new MailboxAddress (senderName, senderEmail));
            // messageToSend.Body = new TextPart (TextFormat.Html) { Text = bodyText };

            // foreach (var toEmail in toEmails)
            //     messageToSend.To.Add (new MailboxAddress (toEmail));

            // if (ccEmails != null)
            //     foreach (var ccEmail in ccEmails)
            //         messageToSend.Cc.Add (new MailboxAddress (ccEmail));

            // using (var smtp = new MailKit.Net.Smtp.SmtpClient ()) {
            //     smtp.AuthenticationMechanisms.Remove ("XOAUTH2");
            //     smtp.MessageSent += async (sender, args) => { // args.Response };
            //         smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

            //         await smtp.ConnectAsync ("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            //         await smtp.AuthenticateAsync (senderEmail, password);
            //         await smtp.SendAsync (messageToSend);
            //         await smtp.DisconnectAsync (true);
            //     };

            //     return isMailSent;
            // }

            #endregion
        }
    }
}