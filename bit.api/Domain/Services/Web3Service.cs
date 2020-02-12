using System;
using System.Threading.Tasks;
using bit.api.Domain.Contracts;
using Nethereum.Web3;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using System.Numerics;
using bit.common.Models;
using Nethereum.RPC.Eth.DTOs;
using System.Linq;

namespace bit.api.Domain.Services
{
    public class Web3Service : IWeb3Service
    {
        Web3 _web3;
        private Contract _countryContract;
        private Contract _tokenContract;
        private readonly string countryContractAddress = "0x3ff08063e2c4199e6462a5e9f7744f95349875a2";
        private readonly string bitCountryTokenAddress = "0xd0deba303954dc5952fbb705a26fb10a73e00d71";
        private string bitCountryCoreAbi = @"[{""constant"":true,""inputs"":[{""name"":""_interfaceID"",""type"":""bytes4""}],""name"":""supportsInterface"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""cfoAddress"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""countries"",""outputs"":[{""name"":""name"",""type"":""string""},{""name"":""theme"",""type"":""uint256""},{""name"":""numberOfBlock"",""type"":""uint256""},{""name"":""reservedBankAddress"",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_tokenId"",""type"":""uint256""}],""name"":""approve"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_countryIndex"",""type"":""uint256""},{""name"":""_index"",""type"":""uint256""}],""name"":""getCountryBlock"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""ceoAddress"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""minimumTokenPurchasedPerBlock"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""residents"",""outputs"":[{""name"":""residentAddress"",""type"":""address""},{""name"":""blockIndex"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""bitCountryWalletAddress"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_newReservedBankWalletAddress"",""type"":""address""},{""name"":""_countryIndex"",""type"":""uint256""}],""name"":""changeReservedBankWalletAddress"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_from"",""type"":""address""},{""name"":""_to"",""type"":""address""},{""name"":""_tokenId"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_newCEO"",""type"":""address""}],""name"":""setCEO"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""getCountryOwnershipCount"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""initialTokenPriceInWei"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""unpause"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_axis"",""type"":""int256""},{""name"":""_yxis"",""type"":""int256""},{""name"":""_countryIndex"",""type"":""uint256""}],""name"":""purchaseBlockToCountry"",""outputs"":[],""payable"":true,""stateMutability"":""payable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_newCFO"",""type"":""address""}],""name"":""setCFO"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""tokenConversionFactor"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""paused"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""countryIndexToApproved"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""countryIndexToCountry"",""outputs"":[{""name"":""name"",""type"":""string""},{""name"":""theme"",""type"":""uint256""},{""name"":""numberOfBlock"",""type"":""uint256""},{""name"":""reservedBankAddress"",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""address""}],""name"":""residentAddressToCountryIndex"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_tokenId"",""type"":""uint256""}],""name"":""ownerOf"",""outputs"":[{""name"":""owner"",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_countryIndex"",""type"":""uint256""}],""name"":""getCountryBlockNumber"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""_owner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""count"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""pause"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeSub"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_to"",""type"":""address""},{""name"":""_tokenId"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_walletAddress"",""type"":""address""}],""name"":""setWalletAddress"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeDiv"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_tokenPriceInWei"",""type"":""uint256""}],""name"":""setTokenPriceInWei"",""outputs"":[{""name"":"""",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalTokenSold"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_name"",""type"":""string""},{""name"":""_theme"",""type"":""uint16""},{""name"":""_owner"",""type"":""address""}],""name"":""createCountry"",""outputs"":[],""payable"":true,""stateMutability"":""payable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalEtherCollected"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""pricePerBlockInWei"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeMul"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeAdd"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""address""}],""name"":""addressToResident"",""outputs"":[{""name"":""residentAddress"",""type"":""address""},{""name"":""blockIndex"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""blocks"",""outputs"":[{""name"":""blockOwner"",""type"":""address""},{""name"":""axis"",""type"":""int256""},{""name"":""yxis"",""type"":""int256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""countryIndexToOwner"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""token"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":"""",""type"":""uint256""}],""name"":""blockIndexToBlock"",""outputs"":[{""name"":""blockOwner"",""type"":""address""},{""name"":""axis"",""type"":""int256""},{""name"":""yxis"",""type"":""int256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""inputs"":[{""name"":""_token"",""type"":""address""},{""name"":""_ceoAddress"",""type"":""address""},{""name"":""_cfoAddress"",""type"":""address""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""_newValue"",""type"":""uint256""}],""name"":""TokensPerEtherUpdated"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""_newAddress"",""type"":""address""}],""name"":""WalletAddressUpdated"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""_beneficiary"",""type"":""address""},{""indexed"":false,""name"":""_cost"",""type"":""uint256""},{""indexed"":false,""name"":""_tokens"",""type"":""uint256""}],""name"":""TokensPurchased"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""from"",""type"":""address""},{""indexed"":false,""name"":""to"",""type"":""address""},{""indexed"":false,""name"":""tokenId"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""owner"",""type"":""address""},{""indexed"":false,""name"":""approved"",""type"":""address""},{""indexed"":false,""name"":""tokenId"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""owner"",""type"":""address""},{""indexed"":false,""name"":""countryId"",""type"":""uint256""},{""indexed"":false,""name"":""blocks"",""type"":""uint256""}],""name"":""BitCountryCreated"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""from"",""type"":""address""},{""indexed"":false,""name"":""to"",""type"":""address""},{""indexed"":false,""name"":""tokenId"",""type"":""uint256""}],""name"":""BitCountryTransfered"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""owner"",""type"":""address""},{""indexed"":false,""name"":""blockNumber"",""type"":""uint256""},{""indexed"":false,""name"":""countryId"",""type"":""uint256""}],""name"":""BitBlockCreated"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":false,""name"":""newContract"",""type"":""address""}],""name"":""ContractUpgrade"",""type"":""event""}]";
        private string bitCountryTokenAbi = @"[{""constant"":true,""inputs"":[],""name"":""name"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""spender"",""type"":""address""},{""name"":""tokens"",""type"":""uint256""}],""name"":""approve"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""totalSupply"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""from"",""type"":""address""},{""name"":""to"",""type"":""address""},{""name"":""tokens"",""type"":""uint256""}],""name"":""transferFrom"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""decimals"",""outputs"":[{""name"":"""",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""tokenOwner"",""type"":""address""}],""name"":""balanceOf"",""outputs"":[{""name"":""balance"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[],""name"":""acceptOwnership"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""owner"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""symbol"",""outputs"":[{""name"":"""",""type"":""string""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeSub"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""to"",""type"":""address""},{""name"":""tokens"",""type"":""uint256""}],""name"":""transfer"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeDiv"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""spender"",""type"":""address""},{""name"":""tokens"",""type"":""uint256""},{""name"":""data"",""type"":""bytes""}],""name"":""approveAndCall"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeMul"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""newOwner"",""outputs"":[{""name"":"""",""type"":""address""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""tokenAddress"",""type"":""address""},{""name"":""amount"",""type"":""uint256""}],""name"":""transferAnyERC20Token"",""outputs"":[{""name"":""success"",""type"":""bool""}],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""tokenOwner"",""type"":""address""},{""name"":""spender"",""type"":""address""}],""name"":""allowance"",""outputs"":[{""name"":""remaining"",""type"":""uint256""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":true,""inputs"":[{""name"":""a"",""type"":""uint256""},{""name"":""b"",""type"":""uint256""}],""name"":""safeAdd"",""outputs"":[{""name"":""c"",""type"":""uint256""}],""payable"":false,""stateMutability"":""pure"",""type"":""function""},{""constant"":true,""inputs"":[],""name"":""getDecimals"",""outputs"":[{""name"":"""",""type"":""uint8""}],""payable"":false,""stateMutability"":""view"",""type"":""function""},{""constant"":false,""inputs"":[{""name"":""_newOwner"",""type"":""address""}],""name"":""transferOwnership"",""outputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""function""},{""inputs"":[],""payable"":false,""stateMutability"":""nonpayable"",""type"":""constructor""},{""payable"":true,""stateMutability"":""payable"",""type"":""fallback""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""from"",""type"":""address""},{""indexed"":true,""name"":""to"",""type"":""address""},{""indexed"":false,""name"":""tokens"",""type"":""uint256""}],""name"":""Transfer"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""tokenOwner"",""type"":""address""},{""indexed"":true,""name"":""spender"",""type"":""address""},{""indexed"":false,""name"":""tokens"",""type"":""uint256""}],""name"":""Approval"",""type"":""event""},{""anonymous"":false,""inputs"":[{""indexed"":true,""name"":""_from"",""type"":""address""},{""indexed"":true,""name"":""_to"",""type"":""address""}],""name"":""OwnershipTransferred"",""type"":""event""}]";
        public Web3Service()
        {
            _web3 = new Web3("https://rinkeby.infura.io");
            _countryContract = _web3.Eth.GetContract(bitCountryCoreAbi, countryContractAddress);
            _tokenContract = _web3.Eth.GetContract(bitCountryTokenAbi, bitCountryTokenAddress);
        }

        public async Task<BitCountryCreatedTransactionStatus> GetCreatingCountryTransactionStatus(string txTran)
        {
            //BitCountryCreated Event Log
            var countryCreatedEvent = _countryContract.GetEvent("BitCountryCreated");
            //Get the receipt of transaction
            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txTran);
            //Create filter input
            var filterInput = countryCreatedEvent.CreateFilterInput(new BlockParameter(receipt.BlockNumber), BlockParameter.CreateLatest());
            var eventLogs = countryCreatedEvent.DecodeAllEventsForEvent<BitCountryCreatedEvent>(receipt.Logs);
            var bitCountryTransactionStatus = new BitCountryCreatedTransactionStatus();
            var status = Convert.ToString(receipt.Status.HexValue);
            if (eventLogs.FirstOrDefault() != null)
            {
                var newCountryEvent = eventLogs.FirstOrDefault().Event;
                bitCountryTransactionStatus.NewCountry = newCountryEvent;
            }
            bitCountryTransactionStatus.IsSuccess = status != "0";
            return bitCountryTransactionStatus;
        }

        public async Task<common.Models.Block> GetBlockUniqueId(int countryContractId, int index)
        {
            const int maxRetries = 2;
            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    var getCountryBlockFunction = _countryContract.GetFunction("getCountryBlock");
                    var countryIndexId = Convert.ToUInt64(countryContractId);
                    var countryBlockIndex = await getCountryBlockFunction.CallAsync<int>(countryIndexId, index).ConfigureAwait(false);
                    var block = await GetBlockDetailFromUniqueId(countryBlockIndex);
                    block.BlockNumber = (int)countryBlockIndex;
                    block.Status = Status.Completed;
                    return block;
                }
                catch (Exception e)
                {
                    continue;
                }
            }

            throw new ApplicationException("Error while retrieving block data from smart contract");
        }

        public async Task<BitBlockCreatedTransactionStatus> GetAddingNewBlockTransactionStatus(string txTran)
        {
            //BitBlockCreated Event Log
            var bitBlockCreatedEvent = _countryContract.GetEvent("BitBlockCreated");
            //Initialize new bitCountryTransactionStatus
            var bitCountryTransactionStatus = new BitBlockCreatedTransactionStatus();
            //Get the receipt of transaction
            var receipt = await _web3.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(txTran);
            if (receipt == null)
            {
                bitCountryTransactionStatus.IsSuccess = false;
                bitCountryTransactionStatus.Error = TransactionStatus.NotFound;
                return bitCountryTransactionStatus;
            }

            //Create filter input
            var filterInput = bitBlockCreatedEvent.CreateFilterInput(new BlockParameter(receipt.BlockNumber), BlockParameter.CreateLatest());
            var eventLogs = bitBlockCreatedEvent.DecodeAllEventsForEvent<BitBlockCreatedEvent>(receipt.Logs);

            var status = Convert.ToString(receipt.Status.HexValue);

            if (eventLogs.FirstOrDefault() != null)
            {
                var newCountryEvent = eventLogs.FirstOrDefault().Event;
                bitCountryTransactionStatus.NewBlock = newCountryEvent;
            }
            else
            {
                bitCountryTransactionStatus.IsSuccess = false;
                bitCountryTransactionStatus.Error = TransactionStatus.NotFound;
                return bitCountryTransactionStatus;
            }
            bitCountryTransactionStatus.IsSuccess = status != "0";

            return bitCountryTransactionStatus;
        }

        public async Task<CountryStructContract> GetCountryDetailFromIndexId(int countryUniqueId)
        {
            try
            {
                var countryIndexToCountryFunction = _countryContract.GetFunction("countryIndexToCountry");
                var country = await countryIndexToCountryFunction.CallDeserializingToObjectAsync<CountryStructContract>(countryUniqueId).ConfigureAwait(false);
                country.TotalBlocks = (int)await GetCountryBlockNumberFromCountryIndex(countryUniqueId);
                return country;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        public async Task<string> GetTokenBalance(string address)
        {
            try
            {
                var getTokenBalanceFunction = _tokenContract.GetFunction("balanceOf");
                var balance = await getTokenBalanceFunction.CallAsync<BigInteger>(address).ConfigureAwait(false);
                return balance.ToString();
            }
            catch (System.Exception e)
            {

                throw;
            }
        }

        private async Task<int> GetCountryBlockNumberFromCountryIndex(int countryUniqueId)
        {
            try
            {
                var getCountryBlockNumberFunction = _countryContract.GetFunction("getCountryBlockNumber");
                var totalBlockNumber = await getCountryBlockNumberFunction.CallAsync<int>(countryUniqueId).ConfigureAwait(false);
                return totalBlockNumber;
            }
            catch (System.Exception e)
            {
                throw;
            }
        }

        private async Task<common.Models.Block> GetBlockDetailFromUniqueId(int blockUniqueId)
        {
            try
            {
                var blockIndexFunction = _countryContract.GetFunction("blockIndexToBlock");
                var block = await blockIndexFunction.CallDeserializingToObjectAsync<common.Models.Block>(blockUniqueId).ConfigureAwait(false);
                return block;
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}