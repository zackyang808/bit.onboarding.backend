using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bit.api.Domain.Contracts;
using bit.common.Commands;
using bit.common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http;
using bit.common.Contracts;
using bit.common.Models;
using Nethereum.Web3;

namespace bit.api.Controllers
{
    [Route("country/[controller]")]
    public class CountryController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly IBlockService _blockService;
        private readonly IResidenceService _residenceService;
        private IRepository<User> _userRepository;
        private readonly IUserAppContext _userAppContext;
        // GET api/values
        public CountryController(
              IAuthenticationService authenticationService,
              ICountryService countryService,
              IBlockService blockService,
              IResidenceService residenceService,
              IRepository<User> userRepository,
              IUserAppContext userAppContext)
        {
            _authenticationService = authenticationService;
            _countryService = countryService;
            _blockService = blockService;
            _residenceService = residenceService;
            _userRepository = userRepository;
            _userAppContext = userAppContext;
        }

        [HttpPost("createCountry")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateCountry([FromBody]CreateCountry newCountry)
        {
            //TODO: Check if Tran# has been used already
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var currentUser = _userRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                    newCountry.OwnerAddress = currentUser.WalletAddress.ToLower();
                    newCountry.CountryOwner = currentUser.NickName;
                    newCountry.UserId = currentUser.Id;
                    newCountry.UserUId = currentUser.UId;
                    var countryCreated = await _countryService.CreateCountry(newCountry);
                    return Json(new { IsSuccess = countryCreated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getCountry")]
        public async Task<IActionResult> GetCountry(string countryId)
        {
            try
            {
                if (string.IsNullOrEmpty(countryId))
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var country = await _countryService.GetCountryById(countryId);
                    return Json(new { IsSuccess = true, Country = country });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getBlocksByCountry")]
        public async Task<IActionResult> GetBlocksByCountry(string countryId)
        {
            try
            {
                if (string.IsNullOrEmpty(countryId))
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var blocks = await _blockService.GetBlocksByCountryId(countryId);
                    return Json(new { IsSuccess = true, Blocks = blocks });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getBlockById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetBlockById(string blockId)
        {
            try
            {
                if (string.IsNullOrEmpty(blockId))
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var blockDetail = await _blockService.GetBlockById(blockId);

                    return Json(new { IsSuccess = blockDetail is object, block = blockDetail });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getSurroundingBlocksByBlock")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSurroundingBlocksByBlock(string blockId, string countryId)
        {
            //TODO: your code here
            try
            {
                if (string.IsNullOrEmpty(blockId) || string.IsNullOrEmpty(countryId))
                {
                    return Json(new { IsSuccess = false, Message = "Invalid request parameters." });
                }

                if (!await _residenceService.GetIsResidentOfCountry(_userAppContext.CurrentUserId,countryId))
                {
                    return Json(new { IsSuccess = false, Message = "The current user is not the resident of the country." });
                }

                else
                {
                    var blocks = await _blockService.GetSurroundingBlocksByBlockId(blockId);

                    return Json(new { IsSuccess = true, Blocks = blocks });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getCountryByBlock")]
        public IActionResult GetCountryByBlock(int? blockNumber)
        {
            try
            {
                if (!blockNumber.HasValue)
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var country = _countryService.GetCountryByBlockNumber(blockNumber.Value);
                    return Json(new { IsSuccess = true, Country = country });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getCountriesByUser")]
        public IActionResult GetCountriesFromUser(string userId)
        {
            try
            {
                var countries = _countryService.GetCountriesByUser(userId);
                return Json(new { IsSuccess = true, Countries = countries });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getCountries")]
        public IActionResult GetCountries()
        {
            try
            {
                var country = _countryService.GetCountries(4, 0);
                return Json(new { IsSuccess = true, Countries = country });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getActivityRules")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetActivityRules(string countryId)
        {
            try
            {
                var currentRules = await _countryService.GetCountryActivityRules(countryId);

                if (currentRules is null)
                {
                    currentRules = new CountryActivityRule
                    {
                        CountryId = countryId,
                        Article = 0,
                        ArticleLike = 0,
                        ArticleView = 0,
                        Story = 0,
                        StoryLike = 0,
                        StoryView = 0,
                        Comment = 0,
                        CommentLike = 0,
                        Share = 0,
                        Referral = 0,
                    };
                }

                return Json(new { IsSuccess = true, rules = currentRules });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("updateActivityRules")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateActivityRules([FromBody]CreateCountryActivityRule activityRules)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var country = await _countryService.GetCountryById(activityRules.CountryId);
                    if (country is null)
                    {
                        return Json(new { IsSuccess = false, Message = "Invalid country specified" });
                    }

                    if (country.OwnerId != _userAppContext.CurrentUserId)
                    {
                        return Json(new { IsSuccess = false, Message = "Not the owner of the country" });
                    }

                    var isUpdated = _countryService.UpdateCountryActivityRules(activityRules);

                    return Json(new { IsSuccess = isUpdated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("purchaseBlock")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> PurchaseBlock([FromBody]CreateBlock newBlock)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var currentUser = _userRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                    newBlock.BlockOwner = currentUser.WalletAddress.ToLower();
                    var blockCreated = _countryService.AddBlockToCountry(newBlock);
                    return Json(new { IsSuccess = blockCreated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("search")]
        public IActionResult Search(int offset)
        {
            try
            {
                var countries = _countryService.GetCountries(8, offset);
                return Json(new { IsSuccess = true, Countries = countries });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getWelcomeMessage")]
        public async Task<IActionResult> GetWelcomeMessage(string countryId)
        {
            try
            {
                if (string.IsNullOrEmpty(countryId))
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var welcomeMessage = _countryService.GetWelcomeMessage(countryId);
                    return Json(new { IsSuccess = true, WelcomeMessage = welcomeMessage });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("updateWelcomeMessage")]
        public IActionResult UpdateWelcomeMessage([FromBody]WelcomeMessage welcomeMessage)
        {
            try
            {
                var response = _countryService.UpdateWelcomeMessage(welcomeMessage.Message, welcomeMessage.CountryId);
                    
                return Json(new { IsSuccess = response });
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }
    }
}
