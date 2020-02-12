using bit.api.Domain.Contracts;
using bit.common.Auth;
using bit.common.Contracts;
using bit.common.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        // These constants may be changed without breaking existing hashes.
        public const int SALT_BYTES = 24;
        public const int HASH_BYTES = 18;
        public const int PBKDF2_ITERATIONS = 64000;

        private IRepository<User> _userRepository;
        private IRepository<Login> _loginRepository;
        private IPasswordStorage _encryptPassword;
        private IJwtHandler _jwtHandler;

        public AuthenticationService(IRepository<User> userRepository,
                                IRepository<Login> loginRepository,
                                IPasswordStorage encryptPassword,
                                IJwtHandler jwtHandler)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _encryptPassword = encryptPassword;
            _jwtHandler = jwtHandler;
        }
        /// <summary>
        /// Register new customer
        /// </summary>
        /// <param name="user"></param>
        public async Task<JsonWebToken> Register(SignUpPersonal user)
        {
            try
            {
                if (user == null) throw new ApplicationException("Incomplete register request - user is null");
                if (user.EmailAddress == null) throw new ApplicationException("Incomplete register request - user's email is null");
                if (user.Password == null || user.Password.Length == 0) throw new ApplicationException("Incomplete register request - Password is null");
                var existingUser = _userRepository.Get(x => x.Login.Username == user.EmailAddress).FirstOrDefault();
                if (existingUser != null)
                {
                    throw new ApplicationException("Email address has been used in registration.");
                }

                // hash password
                var passHash = _encryptPassword.CreateHash(user.Password);

                //var passHash = new PBKDF2(user.Password,SALT_BYTES,PBKDF2_ITERATIONS,"HMACSHA512");
                var UId = Guid.NewGuid();
                var objectId = ObjectId.GenerateNewId().ToString();
                var login = new Login()
                {
                    Id = objectId,
                    UId = UId,
                    Username = user.EmailAddress,
                    PasswordHash = passHash,
                    IsDisabled = true,
                    EmailAddressAuthorized = false,
                    EmailCode = user.EmailCode,
                    ExpiredOn = DateTime.UtcNow.AddHours(24),
                    PasswordFormat = PBKDF2_ITERATIONS,
                    TermsAccepted = user.TermsConditionsAccepted
                };

                var person = new User()
                {
                    Id = objectId,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    MobilePhone = user.MobileNumber,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    UId = UId,
                    Login = login,
                    WalletAddress = user.WalletAddress.ToLower(),
                    NickName = user.NickName
                };

                await _userRepository.Add(person);
                return _jwtHandler.Create(person.Id);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Register error - " + ex.Message);
            }
        }

        /// <summary>
        /// Verify password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> VerifyPassword(string email, string password, string walletAddress)
        {
            //TODO for testing purpose
            if (string.IsNullOrEmpty(password)) throw new ApplicationException("Login fail - Password is null");

            var user = _userRepository.GetQueryable().Where(x => x.Login.Username.Equals(email)).FirstOrDefault();
            if (user == null) throw new ApplicationException("Login fail - user is null");
            if (user.WalletAddress.ToLower() != walletAddress.ToLower()) throw new ApplicationException("Login fail - Wallet address does not match");
            return _encryptPassword.VerifyPassword(password, user.Login.PasswordHash);
        }

        public async Task ResetPassword(Login user, string newPassword)
        {
            if (user == null) throw new ApplicationException("Incomplete reset password - user is null");
            // hash password
            var passHash = _encryptPassword.CreateHash(newPassword);
            user.PasswordHash = passHash;

            await _loginRepository.Update(user);

        }

        public async Task<JsonWebToken> LoginAsync(string email, string password, string walletAddress)
        {
            var user = _userRepository.Get(x => x.Login.Username == email).FirstOrDefault();
            if (user == null)
            {
                throw new ApplicationException("Invalid credentials");
            }
            var passwordCorrect = await VerifyPassword(email, password, walletAddress.ToLower());
            if (!passwordCorrect)
            {
                throw new ApplicationException("Invalid credentials");
            }

            return _jwtHandler.Create(user.Id);
        }

        public bool IsAccountAlreadyCreated(string walletAddress)
        {
            var user = _userRepository.Get(x => x.WalletAddress.ToLower() == walletAddress.ToLower()).FirstOrDefault();
            return user != null;
        }
    }
}
