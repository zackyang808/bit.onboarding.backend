using bit.api.Domain.Contracts;
using bit.common.Auth;
using bit.common.Commands;
using bit.common.Contracts;
using bit.common.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Services
{
    public class ProfileService : IProfileService
    {
        private IRepository<User> _userRepository;
        private IRepository<Login> _loginRepository;
        private IRepository<BlockDetail> _blockRepository;
        private IRepository<Post> _postRepository;
        private IRepository<PostImage> _postImageRepository;
        private IWeb3Service _web3Service;
        public ProfileService(IRepository<User> userRepository,
                                IRepository<Login> loginRepository,
                                IPasswordStorage encryptPassword,
                                IRepository<BlockDetail> blockRepository,
                                IRepository<Post> postRepository,
                                IRepository<PostImage> postImageRepository,
                                IWeb3Service web3Service,
                                IJwtHandler jwtHandler)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _blockRepository = blockRepository;
            _postRepository = postRepository;
            _postImageRepository = postImageRepository;
            _web3Service = web3Service;
        }

        public Task<string> GetTokenBalance(string address)
        {
            return _web3Service.GetTokenBalance(address);
        }
    }
}