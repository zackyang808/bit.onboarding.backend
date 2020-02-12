using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace bit.common.Commands
{
    public class CreateUser
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string NickName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string WalletAddress { get; set; }

    }
}
