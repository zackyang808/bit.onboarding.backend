using bit.common.Commands;
using bit.common.Models;
using bit.common.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace bit.api.Domain.Contracts
{
    public interface ICommentService
    {
        Task<CommentViewModel> CreateComment(string postId, string content, UserViewModel author);
        Task<List<CommentViewModel>> GetCommentsByPostId(string postId, int offset);
        void GetReactionsForComments(string userId, List<CommentViewModel> comments);
        Task<bool> LikeCommentById(string userId, string commentId);
        Task<bool> UnlikeCommentById(string userId, string commentId);
        Task<bool> DislikeCommentById(string userId, string commentId);
        Task<bool> UndislikeCommentById(string userId, string commentId);
    }
}
