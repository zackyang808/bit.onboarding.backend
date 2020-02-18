using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface IResidenceService
    {
        Task<bool> GetIsResidentOfCountry(string userId, string countryId);
    }
}
