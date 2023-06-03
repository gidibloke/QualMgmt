using Application.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEmailer
    {
        Task PrepareAndSendEmail(EmailTypeEnum emailType, EmailMessage message);
    }
}
