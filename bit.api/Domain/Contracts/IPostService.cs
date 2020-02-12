using bit.common.Auth;
using bit.common.Commands;
using bit.common.Models;
using bit.common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface IPostService
    {
        Task<PostViewModel> CreatePost(CreatePost newPost);
        List<PostViewModel> GetPostsAsync(string blockId, int offset);
        PostViewModel GetPost(string postId);
        void GetReactionsForPosts(string userId, List<PostViewModel> posts);
        void GetReactionsForPost(string userId, PostViewModel post);
        Task<bool> LikePostById(string userId, string postId);
        Task<bool> UnlikePostById(string userId, string postId);
        Task<bool> DislikePostById(string userId, string postId);
        Task<bool> UndislikePostById(string userId, string postId);
    }
}
