using bit.api.Domain.Contracts;
using bit.common.Auth;
using bit.common.Commands;
using bit.common.Contracts;
using bit.common.Models;
using bit.common.ViewModels;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Services
{
    public class CountryService : ICountryService
    {
        // These constants may be changed without breaking existing hashes.
        private IRepository<User> _userRepository;
        private IRepository<Login> _loginRepository;
        private IRepository<Country> _countryRepository;
        private IRepository<CountryActivityRule> _countryRuleRepository;
        private IRepository<BlockDetail> _blockRepository;
        private IWeb3Service _web3Service;

        public CountryService(IRepository<User> userRepository,
                                IRepository<Login> loginRepository,
                                IRepository<Country> countryRepository,
                                IRepository<CountryActivityRule> countryRuleRepository,
                                IRepository<BlockDetail> blockRepository,
                                IWeb3Service web3Service
                                )
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _countryRepository = countryRepository;
            _countryRuleRepository = countryRuleRepository;
            _blockRepository = blockRepository;
            _web3Service = web3Service;
        }
        /// <summary>
        /// Create new country
        /// </summary>
        public async Task<bool> CreateCountry(CreateCountry newCountry)
        {
            try
            {
                if (newCountry == null) throw new ApplicationException("New country is not completed");

                if (newCountry.CountryName == null
                || newCountry.CountryDescription == null
                || newCountry.BlockNumber == 0
                || newCountry.Theme == null)
                    throw new ApplicationException("Country detail is not completed");

                var UId = Guid.NewGuid();
                var objectId = ObjectId.GenerateNewId().ToString();
                var newCountryObj = new Country
                {
                    UId = UId,
                    Id = objectId,
                    Name = newCountry.CountryName,
                    Description = newCountry.CountryDescription,
                    President = newCountry.CountryOwner,
                    Theme = newCountry.Theme,
                    BlockNumber = newCountry.BlockNumber,
                    EstimatedValue = ((decimal)newCountry.BlockNumber * (decimal)0.0005) + (decimal)(((decimal)newCountry.BlockNumber * (decimal)0.0005) * 20 / 100),
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = newCountry.UserUId,
                    Population = 256 * newCountry.BlockNumber,
                    CountryIndex = newCountry.CountryIndex,
                    OwnerAddress = newCountry.OwnerAddress,
                    OwnerId = newCountry.UserId,
                    BankBalance = newCountry.BlockNumber * 1000,
                    OriginalValue = ((decimal)newCountry.BlockNumber * (decimal)0.0005),
                    CountryContractUniqueId = newCountry.CountryContractUniqueId,
                    TxTran = newCountry.TxTran,
                    Status = newCountry.Status,
                    TotalBlocks = 9,
                    WelcomeMessage = "Welcome to my country!",
                };

                await _countryRepository.Add(newCountryObj);
                return true;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Create country error" + e.Message);
            }
        }

        public async Task<CountryViewModel> GetCountryById(string countryId)
        {
            try
            {
                var country = _countryRepository.GetByIdAsync(countryId);
                if (country == null)
                    throw new ApplicationException("This country is not available");
                
                var viewCountry = new CountryViewModel
                {
                    Id = country.Id,
                    Name = country.Name,
                    Description = country.Description,
                    CountryIndex = country.CountryIndex,
                    BankBalance = country.BankBalance,
                    CreatedBy = country.CreatedBy,
                    CreatedOn = country.CreatedOn,
                    DigitalAssests = country.DigitalAssets,
                    EstimatedValue = country.EstimatedValue,
                    OriginalValue = country.OriginalValue,
                    OwnerId = country.OwnerId,
                    OwnerAddress = country.OwnerAddress,
                    Population = country.Population,
                    President = country.President,
                    Theme = country.Theme,
                    ThemeImage = country.ThemeImage,
                    Blocks = country.Blocks,
                    TotalBlocks = country.TotalBlocks,
                    TxTran = country.TxTran,
                    UpdatedBy = country.UpdatedBy,
                    UpdatedOn = country.UpdatedOn,
                };

                if (country.Status == Status.Pending && country.TxTran != null)
                {
                    //Change transaction status
                    var transactionStatus = await _web3Service.GetCreatingCountryTransactionStatus(country.TxTran);
                    if (transactionStatus.IsSuccess && transactionStatus.NewCountry != null)
                    {

                        country.Status = Status.Completed;
                        country.CountryIndex = (int)transactionStatus.NewCountry.ContractUniqueCountryId;
                        country.TotalBlocks = (int)transactionStatus.NewCountry.TotalBlocks;

                        //Add blocks to country which are pulled from smart contract - if country status is pending, most likely it hasn't pulled data from blocks
                        var newBlocks = new List<Block>();
                        for (int i = 0; i < country.TotalBlocks; i++)
                        {
                            var countryBlock = await _web3Service.GetBlockUniqueId(country.CountryIndex, i);
                            newBlocks.Add(countryBlock);
                        }
                        country.Blocks = newBlocks;
                        await _countryRepository.Update(country);
                    }
                    else //Transaction is not confirmed or pending - if it's fail the country will be automacally removed
                    {
                    }
                }

                if (country.Status == Status.NewBlockAddedPending)
                {
                    //transaction status change;
                    bool txCompleted = false;

                    var pendingBlocks = country.Blocks.Where(x => x.Status == Status.Pending).ToList();
                    if (pendingBlocks.Any())
                    {
                        foreach (var pendingBlock in pendingBlocks)
                        {
                            txCompleted = false;
                            var transactionStatus = await _web3Service.GetAddingNewBlockTransactionStatus(pendingBlock.TxTran);
                            if (transactionStatus.IsSuccess)
                            {
                                txCompleted = true;
                            }
                            else
                            {
                                txCompleted = false;
                            }
                        }
                    }
                    else
                    {
                        txCompleted = true;
                    }

                    if (txCompleted)
                    {
                        var newBlocks = new List<Block>();

                        //Any block is pending to be added to the country
                        var countryFromContract = await _web3Service.GetCountryDetailFromIndexId(country.CountryIndex);
                        for (int i = 0; i < country.TotalBlocks; i++)
                        {
                            var countryOldBlock = await _web3Service.GetBlockUniqueId(country.CountryIndex, i);
                            newBlocks.Add(countryOldBlock);
                        }
                        country.Blocks = newBlocks;
                        country.Status = Status.Completed;
                        await _countryRepository.Update(country);
                    }
                }

                return viewCountry;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Can not get the country with error " + e.Message);
            }
        }

        public List<CountryViewModel> GetCountriesByUser(string userId)
        {
            try
            {
                var countries = _countryRepository.Get(x => x.OwnerId == userId).Select(x => new CountryViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    BankBalance = x.BankBalance,
                    EstimatedValue = x.EstimatedValue,
                    Population = x.Population,
                    Theme = x.Theme,
                    President = x.President,
                    Id = x.Id,
                    CountryContractUniqueId = x.CountryContractUniqueId,
                    TxTran = x.TxTran,
                    Status = x.Status
                }).ToList();

                return countries;
            }
            catch (System.Exception)
            {

                throw;
            }
        }

        public List<CountryViewModel> GetCountries(int limit, int skip)
        {
            try
            {
                int getLimit = limit == 0 ? 10 : limit;

                var countries = _countryRepository.GetQueryable().Select(x => new CountryViewModel
                {
                    Name = x.Name,
                    Description = x.Description,
                    BankBalance = x.BankBalance,
                    EstimatedValue = x.EstimatedValue,
                    Population = x.Population,
                    Theme = x.Theme,
                    President = x.President,
                    Id = x.Id
                }).OrderByDescending(x => x.Id).Skip(skip).Take(getLimit).ToList();

                return countries;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }

        public bool AddBlockToCountry(CreateBlock block)
        {
            try
            {
                var country = _countryRepository.GetByIdAsync(block.CountryId);

                if (country == null)
                    throw new ApplicationException("This country is not available");

                var newBlock = new Block
                {
                    BlockOwner = block.BlockOwner,
                    BlockNumber = -1, //Will pull data from blockchain when update
                    Axis = block.Axis,
                    YAxis = block.YAxis,
                    Status = Status.Pending,
                    TxTran = block.TxTran
                };

                country.TotalBlocks++;
                country.Status = Status.NewBlockAddedPending;
                country.Blocks.Add(newBlock);
                _countryRepository.Update(country);
            }
            catch (System.Exception)
            {

                throw;
            }

            return true;
        }

        public CountryViewModel GetCountryByBlockNumber(int blockNumber)
        {
            try
            {
                var country = _countryRepository
                    .Get(x => x.Blocks.Any(y => 
                        y.BlockNumber == blockNumber))
                    .FirstOrDefault();
                if (country == null)
                    throw new ApplicationException("This country is not available");

                var viewCountry = new CountryViewModel
                {
                    Id = country.Id,
                    Name = country.Name,
                    Description = country.Description,
                    CountryIndex = country.CountryIndex,
                    BankBalance = country.BankBalance,
                    CreatedBy = country.CreatedBy,
                    CreatedOn = country.CreatedOn,
                    DigitalAssests = country.DigitalAssets,
                    EstimatedValue = country.EstimatedValue,
                    OriginalValue = country.OriginalValue,
                    OwnerId = country.OwnerId,
                    OwnerAddress = country.OwnerAddress,
                    Population = country.Population,
                    President = country.President,
                    Theme = country.Theme,
                    ThemeImage = country.ThemeImage,
                    Blocks = country.Blocks,
                    TotalBlocks = country.TotalBlocks,
                    TxTran = country.TxTran,
                    UpdatedBy = country.UpdatedBy,
                    UpdatedOn = country.UpdatedOn,
                };

                return viewCountry;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Can not get the country with error " + e.Message);
            }
        }

        public string GetWelcomeMessage(string countryId)
        {
            try
            {
                var country = _countryRepository.GetByIdAsync(countryId);
                if (country == null)
                    throw new ApplicationException("This country is not available");

                var welcomeMessage = country.WelcomeMessage;

                return welcomeMessage;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Can not get the welcome message with error " + e.Message);
            }
        }

        public bool UpdateWelcomeMessage(string welcomeMessage, string countryId)
        {
            try
            {
                var country = _countryRepository.GetByIdAsync(countryId);

                if (country == null)
                    throw new ApplicationException("This country is not available");

                country.WelcomeMessage = welcomeMessage;
                _countryRepository.Update(country);

                return true;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException(e.Message);
            }
        }
        public async Task<CountryActivityRule> GetCountryActivityRules(string countryId)
        {
            try
            {
                var currentRules = _countryRuleRepository
                    .Get(x => x.CountryId == countryId
                        && !x.IsDeleted)
                    .SingleOrDefault();

                return currentRules;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error while retrieving country activity rules: " + e.Message);
            }
        }

        public bool UpdateCountryActivityRules(CreateCountryActivityRule activityRules)
        {
            try
            {
                var currentRules = _countryRuleRepository
                    .Get(x => x.CountryId == activityRules.CountryId
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (currentRules is object)
                {
                    currentRules.IsDeleted = true;

                    _countryRuleRepository.Update(currentRules);
                }

                var newRules = new CountryActivityRule
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    CountryId = activityRules.CountryId,
                    Article = activityRules.Article,
                    ArticleLike = activityRules.ArticleLike,
                    ArticleView = activityRules.ArticleView,
                    Story = activityRules.Story,
                    StoryLike = activityRules.StoryLike,
                    StoryView = activityRules.StoryView,
                    Comment = activityRules.Comment,
                    CommentLike = activityRules.CommentLike,
                    Share = activityRules.Share,
                    Referral = activityRules.Referral,
                };

                _countryRuleRepository.Add(newRules);

                return true;
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to update country activity rules: " + e.Message);
            }
        }
    }
}
