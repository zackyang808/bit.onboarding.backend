using bit.common.Auth;
using bit.common.Commands;
using bit.common.Models;
using bit.common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface ICountryService
    {
        Task<bool> CreateCountry(CreateCountry newCountry);
        Task<CountryViewModel> GetCountryById(string countryId);
        List<CountryViewModel> GetCountriesByUser(string userId);
        CountryViewModel GetCountryByBlockNumber(int blockNumber);
        List<CountryViewModel> GetCountries(int limit, int skip);
        bool AddBlockToCountry(CreateBlock block);
        string GetWelcomeMessage(string countryId);
        bool UpdateWelcomeMessage(string welcomeMessage, string countryId);
        Task<CountryActivityRule> GetCountryActivityRules(string countryId);
        bool UpdateCountryActivityRules(CreateCountryActivityRule activityRules);
    }
}
