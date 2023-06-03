using Application.Helpers;
using Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class EmailerService : IEmailer
    {
        public Task PrepareAndSendEmail(EmailTypeEnum emailType, EmailMessage message)
        {
            var email = PrepareAndSendEmail(emailType, message);
            return Task.CompletedTask;
            //throw new NotImplementedException();
        }

        private EmailMessage PrepareEmail(EmailTypeEnum emailType, EmailMessage email)
        {
            
            //Prepare email by reformatting string from a static file or create the string here depending on the value of the enum.
            //return a 
            return email;
        }
    }
}
