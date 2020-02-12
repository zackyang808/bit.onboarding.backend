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
    [Route("profile/[controller]")]
    public class ProfileController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private IProfileService _profileService;
        private IRepository<User> _userRepository;
        private readonly IUserAppContext _userAppContext;
        // GET api/values
        public ProfileController(
              IAuthenticationService authenticationService,
              ICountryService countryService,
              IRepository<User> userRepository,
              IProfileService profileService,
              IUserAppContext userAppContext)
        {
            _authenticationService = authenticationService;
            _countryService = countryService;
            _userRepository = userRepository;
            _profileService = profileService;
            _userAppContext = userAppContext;
        }

        [HttpGet("getTokenBalance")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetTokenBalance()
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
                    var tokenBalance = await _profileService.GetTokenBalance(currentUser.WalletAddress.ToLower());
                    var longValue = Convert.ToDecimal(tokenBalance);
                    if (longValue != 0)
                    {
                        return Json(new { IsSuccess = true, Balance = longValue / 1000000000000000000 });
                    }
                    return Json(new { IsSuccess = true, Balance = 0 });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }
    }
}
