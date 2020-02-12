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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using bit.common.ViewModels;

namespace bit.api.Controllers
{
    [Route("post/[controller]")]
    public class PostController : Controller
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly IBlockService _blockService;
        private IRepository<User> _userRepository;
        private IRepository<BlockDetail> _blockRepository;
        private IRepository<PostImage> _postImageRepository;
        private IPostService _postService;
        private readonly ICommentService _commentService;
        private readonly IUserAppContext _userAppContext;
        private readonly IHostingEnvironment _environment;
        private readonly IFileService _fileService;
        private const string _postPhotoPath = "images/";
        // GET api/values
        public PostController(
              IAuthenticationService authenticationService,
              ICountryService countryService,
              IBlockService blockService,
              IRepository<User> userRepository,
              IRepository<BlockDetail> blockRepository,
              IRepository<PostImage> postImageRepository,
              IPostService postService,
              ICommentService commentService,
              IHostingEnvironment environment,
              IFileService fileService,
              IUserAppContext userAppContext)
        {
            _authenticationService = authenticationService;
            _countryService = countryService;
            _blockService = blockService;
            _userRepository = userRepository;
            _blockRepository = blockRepository;
            _postImageRepository = postImageRepository;
            _postService = postService;
            _commentService = commentService;
            _environment = environment;
            _fileService = fileService;
            _userAppContext = userAppContext;
        }

        [HttpPost("createTopic")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateTopic([FromBody]CreateTopic newTopic)
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
                    newTopic.OwnerId = currentUser.Id;
                    newTopic.UserUId = currentUser.UId;
                    var topicCreated = await _blockService.CreateTopic(newTopic);
                    return Json(new { IsSuccess = true, Topic = topicCreated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("updatePhoto")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> UpdatePostPhoto(string blockId)
        {
            IFormFile file = Request.Form.Files[0];
            var fileExtension = Path.GetExtension(file.FileName);
            List<string> acceptedExtensions = new List<string> { ".jpg", ".png", ".gif", ".jpeg" };
            string postPhotoFilePath = Path.Combine(_environment.ContentRootPath, _postPhotoPath);

            if (fileExtension != null && !acceptedExtensions.Contains(fileExtension.ToLower()))
            {
                return Json(new { Success = false, Message = "Supported file types are *.jpg, *.png, *.gif, *.jpeg" });
            }
            else
            {

                var newFileName = await _fileService.SaveFile(file, FileType.PostPhoto);
                var newUrl = await _fileService.GetFilePath(newFileName, FileType.PostPhoto);

                try
                {
                    var newPostPhoto = new PostImage
                    {
                        BlockId = blockId,
                        FileName = newFileName,
                        ImageUrl = newUrl,
                        CreatedOn = DateTime.Now,
                        CreatedBy = _userAppContext.CurrentUserId,
                        UpdatedOn = DateTime.Now,
                        UpdatedBy = _userAppContext.CurrentUserId,
                        IsDeleted = false
                    };
                    await _postImageRepository.Add(newPostPhoto);
                    return Json(new { Success = true, newProfilePhotoName = newFileName, newUrl });
                }
                catch (Exception e)
                {
                    return Json(new { Success = false, Message = "Error while uploading Profile Photo" });
                }
            }
        }

        [HttpPost("createNewPost")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreatePost([FromBody]CreatePost newPost)
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

                    newPost.OwnerId = currentUser.Id;
                    newPost.UserUId = currentUser.UId;
                    newPost.OwnerWalletAddress = currentUser.WalletAddress.ToLower();

                    var postCreated = await _postService.CreatePost(newPost);

                    return Json(new { IsSuccess = true, Post = postCreated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("getPosts")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPosts([FromBody]GetPostModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.BlockId))
                {
                    return Json(new { IsSuccess = false, Message = "Block is empty" });
                }
                else
                {
                    var currentUser = _userRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                    //Check if current user is a resident of this country or this country allows public access

                    var posts = _postService.GetPostsAsync(model.BlockId, model.OffSet);
                    _postService.GetReactionsForPosts(_userAppContext.CurrentUserId, posts);

                    return Json(new { IsSuccess = true, Posts = posts });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getPostById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetPostById(string postId)
        {
            try
            {
                if (string.IsNullOrEmpty(postId))
                {
                    return Json(new { IsSuccess = false, Message = "Post ID is empty" });
                }
                else
                {
                    var currentUser = _userRepository.GetByIdAsync(_userAppContext.CurrentUserId);
                    //Check if current user is a resident of this country or this country allows public access

                    var post = _postService.GetPost(postId);
                    _postService.GetReactionsForPost(_userAppContext.CurrentUserId, post);

                    return Json(new { IsSuccess = true, Post = post });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("likePostById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LikePostById([FromBody]CreatePostActivity post)
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

                    //TODO: Return message on failure?
                    var success = await _postService.LikePostById(currentUser.Id, post.PostId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("unlikePostById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UnlikePostById([FromBody]CreatePostActivity post)
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

                    //TODO: Return message on failure?
                    var success = await _postService.UnlikePostById(currentUser.Id, post.PostId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("dislikePostById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DislikePostById([FromBody]CreatePostActivity post)
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

                    //TODO: Return message on failure?
                    var success = await _postService.DislikePostById(currentUser.Id, post.PostId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("undislikePostById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UndislikePostById([FromBody]CreatePostActivity post)
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

                    //TODO: Return message on failure?
                    var success = await _postService.UndislikePostById(currentUser.Id, post.PostId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("createNewComment")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateComment([FromBody]CreateComment newComment)
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

                    var author = new UserViewModel
                    {
                        Id = currentUser.Id,
                        NickName = currentUser.NickName,
                        ProfileImageUrl = currentUser.ProfileImageUrl,
                        WalletAddress = currentUser.WalletAddress
                    };

                    var commentCreated = await _commentService.CreateComment(newComment.PostId, newComment.Content, author);

                    return Json(new { IsSuccess = true, Topic = commentCreated });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpGet("getCommentsByPostId")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetCommentsByPostId(string postId, int offset)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { IsSuccess = false, Message = "" });
                }
                else
                {
                    var comments = await _commentService.GetCommentsByPostId(postId, offset);
                    _commentService.GetReactionsForComments(_userAppContext.CurrentUserId, comments);

                    return Json(new { IsSuccess = true, comments });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("likeCommentById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> LikeCommentById([FromBody]CreateCommentActivity comment)
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

                    //TODO: Return message on failure?
                    var success = await _commentService.LikeCommentById(currentUser.Id, comment.CommentId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("unlikeCommentById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UnlikeCommentById([FromBody]CreateCommentActivity comment)
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

                    //TODO: Return message on failure?
                    var success = await _commentService.UnlikeCommentById(currentUser.Id, comment.CommentId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("dislikeCommentById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DislikeCommentById([FromBody]CreateCommentActivity comment)
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

                    //TODO: Return message on failure?
                    var success = await _commentService.DislikeCommentById(currentUser.Id, comment.CommentId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }

        [HttpPost("undislikeCommentById")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UndislikeCommentById([FromBody]CreateCommentActivity comment)
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

                    //TODO: Return message on failure?
                    var success = await _commentService.UndislikeCommentById(currentUser.Id, comment.CommentId);

                    return Json(new { IsSuccess = success, Message = "" });
                }
            }
            catch (Exception e)
            {
                return Json(new { IsSuccess = false, Message = e.Message });
            }
        }
    }
}
