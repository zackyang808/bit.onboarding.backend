using System;
using System.Collections.Generic;
using System.Text;

namespace bit.common.Models
{
    public class SignUpPersonal
    {
        public SignUpPersonal()
        {
            SendEmailVerificationEmail = true;
            SendWelcomeMail = true;
        }
        public Guid Id { get; set; } // TODO: not in use
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }
        public string DialCode { get; set; }
        public int GenderId { get; set; }
        public bool ExistingAccount { get; set; }
        public bool TermsConditionsAccepted { get; set; }
        public bool SendEmailVerificationEmail { get; set; }
        public bool SendWelcomeMail { get; set; }
        public Guid EmailCode { get; set; }
        public string WalletAddress { get; set; }
        public string NickName { get; set; }
    }
}
