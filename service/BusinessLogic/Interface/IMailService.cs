using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogic.Interface
{
    public interface IMailService
    {
        Task<bool> SendMailAsync(string toEmail, string bodyText,
           string subjectText, List<string> ccEmails = null);
    }
}
