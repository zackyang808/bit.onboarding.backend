using bit.common.Email.Contracts;
using bit.common.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Email.Services
{
    public class EmailService : IEmailService
    {
        private SendGridOption _sendGridConfig;
        public EmailService(IOptions<SendGridOption> sendGridConfig)
        {
            _sendGridConfig = sendGridConfig.Value;
        }

        async public Task<bool> SendEmailMessageAsync(EmailMessage message, string templateId)
        {
            try
            {
                var client = new SendGridClient(_sendGridConfig.SendGridApiKey);

                var sendGridMsg = new SendGridMessageRequest();
                sendGridMsg.Template_id = templateId;
                sendGridMsg.From = new Recipient { Email = "noreply@bit.country", Name = "Bit.Country" };
                sendGridMsg.Personalizations = new List<Personalization>
                {
                    new Personalization
                    {
                        Subject = message.Subject,
                        To = new List<Recipient>
                        {
                            new Recipient
                            {
                                Email = message.Destination,
                                Name = message.FirstName
                            },
                        },
                        dynamic_template_data = message.PresetSubstitutions
                    }
                };

                sendGridMsg.Content = new List<Content>()
                {
                    new Content
                    {
                        Value = ".",
                        Type = "text/html"
                    }
                };

                var jsonString = JsonConvert.SerializeObject(sendGridMsg).ToString();
                var response = await client.RequestAsync(method: SendGridClient.Method.POST,
                                                             requestBody: jsonString,
                                                             urlPath: "mail/send");

                return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted;
            }
            catch (Exception e)
            {
                return false;
            }

        }

        /// <summary>
        /// Send user invitation email async example. This method will be executed from controller or service which implemented IEmailService
        /// If this method is not found on IEmailService, make sure you add the method in the IEmailService
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        async public Task<bool> SendUserInvitationEmailAysnc(EmailMessage message)
        {
            //TODO Please read the instruction below, the Step 1 and 2 are already done as an example for future use
            //TODO Step 1: Add templateId into appsettings.json
            //TODO Step 2: Add record to Common > Email > SendGridOption > public string SendGridUserInvitationTemplate or whatever the name of the template { get; set; } 
            //TODO Step 3: Use SendEmailMessageAsync and pass message, templateId (_sendGridConfig.Name of the new property in SendGridOption you just added)
            //TODO Return true or false, please follow example of SendEmailMessageAsync

            try
            {
                var client = new SendGridClient(_sendGridConfig.SendGridApiKey);

                var response = await SendEmailMessageAsync(message, _sendGridConfig.SendGridUserInvitationTemplate);

                return response;
            }
            catch (Exception e)
            {
                return false;
            }
            //return true; //For demo purpose only
        }
    }


    //Will be needed for creating template//
    public class SendGridMessageRequest
    {
        [JsonProperty("template_id")]
        public string Template_id { get; set; }
        [JsonProperty("from")]
        public Recipient From { get; set; }
        [JsonProperty("personalizations")]
        public List<Personalization> Personalizations { get; set; }
        [JsonProperty("content")]
        public List<Content> Content { get; set; }
    }

    public class Recipient
    {
        public string Email { get; set; }
        public string Name { get; set; }
    }

    public class Personalization
    {
        public Dictionary<string, string> dynamic_template_data { get; set; }
        public List<Recipient> To { get; set; }
        public List<Recipient> Cc { get; set; }
        public List<Recipient> Bcc { get; set; }
        public string Subject { get; set; }
    }

    public class SendGridBaseRequest
    {
        public string EmailAddress { get; set; }
        public string FullName { get; set; }
        public string Subject { get; set; }
    }

    public class SendGridPostScheduleRequest : SendGridBaseRequest
    {
        public string PostSubject { get; set; }
        public string PostBody { get; set; }
        public string PostUrl { get; set; }
    }

    public class Content
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }
}
