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
    public class BlockService : IBlockService
    {
        private IRepository<User> _userRepository;
        private IRepository<Login> _loginRepository;
        private readonly IRepository<Country> _countryRepository;
        private IRepository<BlockDetail> _blockRepository;
        public BlockService(IRepository<User> userRepository,
                                IRepository<Login> loginRepository,
                                IPasswordStorage encryptPassword,
                                IRepository<Country> countryRepository,
                                IRepository<BlockDetail> blockRepository,
                                IJwtHandler jwtHandler)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _countryRepository = countryRepository;
            _blockRepository = blockRepository;
        }

        public async Task<BlockViewModel> CreateTopic(CreateTopic newTopic)
        {
            try
            {
                if (newTopic == null || newTopic.BlockNumber == -1) throw new ApplicationException("Topic is not null");
                if (
                    string.IsNullOrEmpty(newTopic.CountryName) ||
                    string.IsNullOrEmpty(newTopic.Description) ||
                    string.IsNullOrEmpty(newTopic.CountryName))
                    throw new ApplicationException("Topic detail is not completed to save");

                //Make sure only 1 topic per block
                var alreadyHasTopic = _blockRepository.Get(x => x.BlockNumber == newTopic.BlockNumber).Any();

                if (alreadyHasTopic)
                    throw new ApplicationException("This block number already has topic");

                // Find the block this new topic will belong too.
                var owningCountry = _countryRepository
                    .GetQueryable()
                    .Single(x => x.Blocks.Any(y => y.BlockNumber == newTopic.BlockNumber));
                var countryBlock = owningCountry.Blocks.Single(y => y.BlockNumber == newTopic.BlockNumber);

                var objectId = countryBlock.Id;
                var UId = countryBlock.UId;

                // TODO: Consider refactor for BlockDetail and BlockViewModel for interacting with BlockService.
                var blockObj = new BlockDetail
                {
                    UId = UId,
                    Id = objectId,
                    Name = newTopic.Name,
                    Description = newTopic.Description,
                    BlockAxis = newTopic.BlockAxis,
                    BlockYxis = newTopic.BlockYxis,
                    BlockNumber = newTopic.BlockNumber,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = newTopic.UserUId,
                    CountryId = newTopic.CountryId,
                    OwnerId = newTopic.OwnerId,
                    TotalResidents = 1,
                    IsDeleted = false,
                };

                await _blockRepository.Add(blockObj);

                var blockViewModel = new BlockViewModel
                {
                    Id = blockObj.Id,
                    Name = blockObj.Name,
                    Description = blockObj.Description,
                    BlockAxis = blockObj.BlockAxis,
                    BlockYxis = blockObj.BlockYxis,
                    CreatedOn = blockObj.CreatedOn,
                    CreatedBy = blockObj.CreatedBy,
                    Owner = null,
                    TotalResidents = blockObj.TotalResidents,
                };

                return blockViewModel;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Create topic error " + e.Message);
            }
        }

        public async Task<List<BlockViewModel>> GetBlocksByCountryId(string countryId)
        {
            try
            {
                var blocks = _blockRepository
                    .Get(x => x.CountryId == countryId.ToLower())
                    .Select(x => new BlockViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Owner = new UserViewModel
                        {
                            WalletAddress = x.OwnerAddress
                        },
                        TotalResidents = x.TotalResidents,
                        BlockAxis = x.BlockAxis,
                        BlockYxis = x.BlockYxis,
                        CreatedOn = x.CreatedOn,
                        CreatedBy = x.CreatedBy,
                        UpdatedOn = x.UpdatedOn,
                        UpdatedBy = x.UpdatedBy,
                    })
                    .ToList();

                return blocks;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<BlockViewModel> GetBlockById(string blockId)
        {
            try
            {
                var block = _blockRepository
                    .Get(x => x.Id == blockId)
                    .Select(x => new BlockViewModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            Owner = new UserViewModel
                            {
                                WalletAddress = x.OwnerAddress
                            },
                            TotalResidents = x.TotalResidents,
                            BlockAxis = x.BlockAxis,
                            BlockYxis = x.BlockYxis,
                            CreatedOn = x.CreatedOn,
                            CreatedBy = x.CreatedBy,
                            UpdatedOn = x.UpdatedOn,
                            UpdatedBy = x.UpdatedBy,
                        })
                    .SingleOrDefault();

                return block;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<BlockViewModel>> GetSurroundingBlocksByBlockId(string blockId)
        {
            //TODO: your code here

            return new List<BlockViewModel>(); //Replace this with real return values
        }
    }
}
