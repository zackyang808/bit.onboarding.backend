using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bit.common.Models;
using bit.common.Email.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace bit.api.Controllers
{
    [Route("email/[controller]")]
    public class EmailController : Controller
    {
        private readonly IEmailService _emailService;

        public EmailController(
            IEmailService emailService
        )
        {
            _emailService = emailService;
        }

        [HttpPost("sendInvitationEmail")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> SendInvitationEmail([FromBody]Invitation invitation)
        {
            try
            {
                var presetSubstitutions = new Dictionary<string, string>()
                {
                    {"link", invitation.InvitationLink }
                };

                string[] emailAddresses = System.Text.RegularExpressions.Regex.Split(invitation.EmailAddresses.Trim(), ",");

                foreach (var address in emailAddresses)
                {
                    var message = new EmailMessage()
                    {
                        Destination = address.Trim(),
                        PresetSubstitutions = presetSubstitutions,
                    };

                    Task task = new Task(() =>
                    {
                        var res = _emailService.SendUserInvitationEmailAysnc(message);
                    });
                    task.Start();
                }
                
                return Json(new { IsSuccess = true });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }

        }
    }
}
