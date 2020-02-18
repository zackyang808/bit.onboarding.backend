using bit.api.Domain.Contracts;
using bit.common.Contracts;
using bit.common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Services
{
    public class ResidenceService : IResidenceService
    {
        private readonly IRepository<CountryResident> _residenceRepository;
        private readonly IRepository<Country> _countryRepository;

        public ResidenceService(
            IRepository<CountryResident> residenceRepository,
            IRepository<Country> countryRepository)
        {
            _residenceRepository = residenceRepository;
            _countryRepository = countryRepository;
        }

        public async Task<bool> GetIsResidentOfCountry(string userId, string countryId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("userId is required");
            }

            if (string.IsNullOrWhiteSpace(countryId))
            {
                throw new ArgumentException("countryId is required");
            }

            try
            {
                var residency = _residenceRepository
                    .Get(x => x.ResidentId == userId.ToLower()
                        && x.CountryId == countryId.ToLower()
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (residency is object)
                {
                    return true;
                }


                var owner = _countryRepository
                    .Get(x => x.OwnerId == userId.ToLower()
                        && !x.IsDeleted)
                    .Any();

                return owner;
            }
            catch
            {
                throw;
            }
        }
    }
}
