using System.Numerics;
using System.Threading.Tasks;
using bit.common.Models;

namespace bit.api.Domain.Contracts
{
    public interface IWeb3Service
    {
        Task<BitCountryCreatedTransactionStatus> GetCreatingCountryTransactionStatus(string txTran);

        Task<common.Models.Block> GetBlockUniqueId(int countryContractId, int index);
        Task<BitBlockCreatedTransactionStatus> GetAddingNewBlockTransactionStatus(string txTran);
        Task<CountryStructContract> GetCountryDetailFromIndexId(int countryUniqueId);
        Task<string> GetTokenBalance(string address);
    }
}