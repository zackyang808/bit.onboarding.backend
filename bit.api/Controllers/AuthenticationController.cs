using bit.api.Domain.Contracts;
using bit.api.Domain.Services;
using bit.common.Commands;
using bit.common.Contracts;
using bit.common.Models;
using bit.common.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace bit.api.Controllers
{
    [Route("authentication/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IUserAppContext _userAppContext;
        private readonly IRepository<User> _userRepository;
        public AuthenticationController(
              IAuthenticationService authenticationService,
              IRepository<User> userRepository,
              IUserAppContext userAppContext)
        {
            _authenticationService = authenticationService;
            _userRepository = userRepository;
            _userAppContext = userAppContext;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody]CreateUser command)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { IsSuccess = false, Message = "Parameter can not be null" });
                }

                var authenticatedToken = await _authenticationService.Register(new SignUpPersonal
                {
                    FirstName = command.FirstName,
                    LastName = command.LastName,
                    EmailAddress = command.Email,
                    Password = command.Password,
                    WalletAddress = command.WalletAddress.ToLower(),
                    TermsConditionsAccepted = true,
                    NickName = command.NickName
                });

                return Json(new { IsSuccess = true, Token = authenticatedToken });
            }
            catch (ApplicationException e)
            {
                return Json(new { IsSuccess = false, e.Message });
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody]CreateUser command)
        {
            try
            {
                var authenticateUser = await _authenticationService.LoginAsync(command.Email, command.Password, command.WalletAddress.ToLower());
                return Json(new { IsSuccess = true, Token = authenticateUser });
            }
            catch (ApplicationException e)
            {
                return Json(new { IsSuccess = false, e.Message });
            }
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("isAuthenticated")]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("getUser")]
        public IActionResult GetUser()
        {
            try
            {
                var userId = _userAppContext.CurrentUserId;
                var currentUser = _userRepository.GetByIdAsync(userId);
                return Json(new {IsSuccess = true, User = currentUser });
            }
            catch (ApplicationException e)
            {
                return Json(new {IsSuccess = false });
            }
        }

        [HttpPost("checkAccountCreated")]
        public IActionResult CheckIfAccountCreated([FromBody]string walletAddress)
        {
            try
            {
                var accountCreated = _authenticationService.IsAccountAlreadyCreated(walletAddress.ToLower());
                return Json(new { AccountCreated = accountCreated});
            }
            catch (ApplicationException e)
            {
                return Json(new { AccountCreated = false, Message = e.Message });
            }
        }
    }
}
