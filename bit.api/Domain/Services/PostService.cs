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
    public class PostService : IPostService
    {
        private IRepository<User> _userRepository;
        private IRepository<Login> _loginRepository;
        private IRepository<BlockDetail> _blockRepository;
        private IRepository<Post> _postRepository;
        private IRepository<PostImage> _postImageRepository;
        private readonly IRepository<PostLike> _postLikeRepository;
        public PostService(IRepository<User> userRepository,
                                IRepository<Login> loginRepository,
                                IPasswordStorage encryptPassword,
                                IRepository<BlockDetail> blockRepository,
                                IRepository<Post> postRepository,
                                IRepository<PostImage> postImageRepository,
                                IRepository<PostLike> postLikeRepository,
                                IJwtHandler jwtHandler)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
            _blockRepository = blockRepository;
            _postRepository = postRepository;
            _postImageRepository = postImageRepository;
            _postLikeRepository = postLikeRepository;
        }

        public async Task<PostViewModel> CreatePost(CreatePost newPost)
        {
            try
            {

                // TODO: Sanitise content and other input
                if (newPost == null || newPost.BlockNumber == -1) throw new ApplicationException("Create post is invalid");
                if (
                    string.IsNullOrEmpty(newPost.Title) ||
                    string.IsNullOrEmpty(newPost.Content) ||
                    string.IsNullOrEmpty(newPost.OwnerWalletAddress) ||
                    string.IsNullOrEmpty(newPost.BlockId))
                    throw new ApplicationException("Post detail is not completed to save");

                var UId = Guid.NewGuid();
                var objectId = ObjectId.GenerateNewId().ToString();
                var owner = _userRepository.GetByIdAsync(newPost.OwnerId);

                if (owner == null)
                    throw new ApplicationException("Owner is not found on system");

                // TODO: Consider refactor for Post and PostViewModel when interacting with PostService.
                var postObj = new Post
                {
                    UId = UId,
                    Id = objectId,
                    Title = newPost.Title,
                    Summary = newPost.Summary,
                    Content = newPost.Content,
                    CountryId = newPost.CountryId,
                    BlockId = newPost.BlockId,
                    BlockNumber = newPost.BlockNumber,
                    CreatedOn = DateTime.UtcNow,
                    IsDeleted = false,
                    Status = PostStatus.Active,
                    Owner = new UserViewModel
                    {
                        Id = owner.Id,
                        WalletAddress = owner.WalletAddress,
                        NickName = owner.NickName,
                        ProfileImageUrl = owner.ProfileImageUrl
                    },
                    PostType = Enum.Parse<PostType>(newPost.PostType)
                };

                await _postRepository.Add(postObj);
                if (newPost.Images != null && newPost.Images.Any())
                {
                    foreach (var img in newPost.Images)
                    {
                        var imageObj = _postImageRepository.Get(x => x.ImageUrl == img).FirstOrDefault();
                        if (imageObj != null)
                        {
                            imageObj.PostId = objectId;
                        }
                        await _postImageRepository.Update(imageObj);
                    }

                    postObj.FeaturedImage = newPost.Images.FirstOrDefault();
                    await _postRepository.Update(postObj);
                }

                var postViewModel = new PostViewModel
                {
                    Id = postObj.Id,
                    Title = postObj.Title,
                    Summary = postObj.Summary,
                    Content = postObj.Content,
                    CountryId = postObj.CountryId,
                    BlockId = postObj.BlockId,
                    CreatedOn = postObj.CreatedOn,
                    Status = postObj.Status,
                    Owner = postObj.Owner,
                    PostType = postObj.PostType
                };

                return postViewModel;
            }
            catch (System.Exception e)
            {
                throw new ApplicationException("Create post error " + e.Message);
            }
        }

        public PostViewModel GetPost(string postId)
        {
            try
            {
                if (string.IsNullOrEmpty(postId))
                    throw new ApplicationException("Post Id is empty");

                var post = _postRepository
                    .Get(x => x.Id == postId.ToLower())
                    .Select(x => new PostViewModel
                    {
                        Id = x.Id,
                        CountryId = x.CountryId,
                        BlockId = x.BlockId,
                        Owner = x.Owner,
                        Title = x.Title,
                        Summary = x.Summary,
                        Content = x.Content,
                        FeaturedImage = x.FeaturedImage,
                        Status = x.Status,
                        PostType = x.PostType,
                        CreatedOn = x.CreatedOn,
                        UpdatedOn = x.UpdatedOn,
                        UpdatedBy = x.UpdatedBy,
                    })
                    .SingleOrDefault();
                if (post == null)
                    throw new ApplicationException("Post is empty");

                return post;

            }
            catch (System.Exception e)
            {

                throw e;
            }
        }

        public List<PostViewModel> GetPostsAsync(string blockId, int offset)
        {
            try
            {
                var posts = _postRepository
                    .Get(x => x.BlockId == blockId)
                    .OrderByDescending(x => x.CreatedOn)
                    .Skip(offset)
                    .Take(5)
                    .Select(x => new PostViewModel
                        {
                            Id = x.Id,
                            CountryId = x.CountryId,
                            BlockId = x.BlockId,
                            Owner = x.Owner,
                            Title = x.Title,
                            Summary = x.Summary,
                            Content = x.Content,
                            FeaturedImage = x.FeaturedImage,
                            Status = x.Status,
                            PostType = x.PostType,
                            CreatedOn = x.CreatedOn,
                            UpdatedOn = x.UpdatedOn,
                            UpdatedBy = x.UpdatedBy,
                        })
                    .ToList();

                return posts;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void GetReactionsForPosts(string userId, List<PostViewModel> posts)
        {
            try
            {
                posts.ForEach(x =>
                {
                    var reaction = _postLikeRepository
                        .Get(y => y.PostId == x.Id
                            && y.UserId == userId
                            && !y.IsDeleted)
                        .FirstOrDefault();

                    if (reaction is object)
                    {
                        x.isLiked = reaction.Type == PostLikeType.Like;
                        x.isDisliked = reaction.Type == PostLikeType.Dislike;
                    }
                });

                return;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public void GetReactionsForPost(string userId, PostViewModel post)
        {
            try
            {
                var reaction = _postLikeRepository
                    .Get(x => x.PostId == post.Id
                        && x.UserId == userId
                        && !x.IsDeleted)
                    .FirstOrDefault();

                if (reaction is object)
                {
                    post.isLiked = reaction.Type == PostLikeType.Like;
                    post.isDisliked = reaction.Type == PostLikeType.Dislike;
                }

                return;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> LikePostById(string userId, string postId)
        {
            try
            {
                var currentPostLike = _postLikeRepository
                    .Get(x =>
                        x.UserId == userId.ToLower()
                        && x.PostId == postId.ToLower()
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (currentPostLike is object)
                {
                    if (currentPostLike.Type == PostLikeType.Like)
                    {
                        return true;
                    }
                    else
                    {
                        currentPostLike.IsDeleted = true;

                        //TODO: Can this be batched?
                        await _postLikeRepository.Update(currentPostLike);
                    }
                }

                var postLike = new PostLike
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UId = Guid.NewGuid(),
                    UserId = userId,
                    PostId = postId,
                    Type = PostLikeType.Like
                };

                await _postLikeRepository.Add(postLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UnlikePostById(string userId, string postId)
        {
            try
            {
                var postLike = _postLikeRepository
                    .Get(x =>
                        x.UserId == userId.ToLower()
                        && x.PostId == postId.ToLower()
                        && x.Type == PostLikeType.Like
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (postLike is null)
                {
                    return true;
                }

                postLike.IsDeleted = true;

                await _postLikeRepository.Update(postLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> DislikePostById(string userId, string postId)
        {
            try
            {
                var currentPostLike = _postLikeRepository
                    .Get(x =>
                        x.UserId == userId.ToLower()
                        && x.PostId == postId.ToLower()
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (currentPostLike is object)
                {
                    if (currentPostLike.Type == PostLikeType.Dislike)
                    {
                        return true;
                    }
                    else
                    {
                        currentPostLike.IsDeleted = true;

                        //TODO: Can this be batched?
                        await _postLikeRepository.Update(currentPostLike);
                    }
                }

                var postLike = new PostLike
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UId = Guid.NewGuid(),
                    UserId = userId,
                    PostId = postId,
                    Type = PostLikeType.Dislike
                };

                await _postLikeRepository.Add(postLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UndislikePostById(string userId, string postId)
        {
            try
            {
                var postLike = _postLikeRepository
                    .Get(x =>
                        x.UserId == userId.ToLower()
                        && x.PostId == postId.ToLower()
                        && x.Type == PostLikeType.Dislike
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (postLike is null)
                {
                    return true;
                }

                postLike.IsDeleted = true;

                await _postLikeRepository.Update(postLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
