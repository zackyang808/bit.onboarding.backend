using bit.common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Email.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendEmailMessageAsync(EmailMessage message, string templateId);
        Task<bool> SendUserInvitationEmailAysnc(EmailMessage message);
    }
}
