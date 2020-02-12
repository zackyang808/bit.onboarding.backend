using System.Numerics;
using Nethereum.ABI.FunctionEncoding.Attributes;

namespace bit.common.Models
{
    public class BitCountryCreatedEvent
    {
        [Parameter("address", "owner", 1, false)]
        public string OwnerAddress { get; set; }
        [Parameter("uint256", "countryId", 2, false)]
        public BigInteger ContractUniqueCountryId { get; set; }
        [Parameter("uint256", "block", 3, false)]
        public BigInteger TotalBlocks { get; set; }
    }

    public class BitCountryCreatedTransactionStatus
    {
        public BitCountryCreatedEvent NewCountry { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class BitBlockCreatedEvent
    {
        [Parameter("address", "owner", 1, false)]
        public string OwnerAddress { get; set; }
        [Parameter("uint256", "blockNumber", 2, false)]
        public BigInteger BlockIndex { get; set; }
        [Parameter("uint256", "countryId", 3, false)]
        public BigInteger CountryId { get; set; }
    }

    public class BitBlockCreatedTransactionStatus
    {
        public BitBlockCreatedEvent NewBlock { get; set; }
        public bool IsSuccess { get; set; }
        public string Error { get; set; }
    }
}