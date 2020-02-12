using bit.common.Auth;
using bit.common.Commands;
using bit.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface IProfileService
    {
        Task<string> GetTokenBalance(string address);
    }
}
