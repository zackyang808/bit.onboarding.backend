using bit.common.Auth;
using bit.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface IAuthenticationService
    {
        Task<JsonWebToken> Register(SignUpPersonal user);
        Task<bool> VerifyPassword(string email, string password, string walletAddress);
        Task ResetPassword(Login user, string newPassword);
        Task<JsonWebToken> LoginAsync(string email, string password, string walletAddress);
        bool IsAccountAlreadyCreated(string walletAddress);
    }
}
