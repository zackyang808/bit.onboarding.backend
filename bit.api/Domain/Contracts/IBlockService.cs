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
    public interface IBlockService
    {
        Task<BlockViewModel> CreateTopic(CreateTopic newTopic);
        Task<List<BlockViewModel>> GetBlocksByCountryId(string countryId);
        Task<BlockViewModel> GetBlockById(string blockId);
        Task<List<BlockViewModel>> GetSurroundingBlocksByBlockId(string blockId);
    }
}
