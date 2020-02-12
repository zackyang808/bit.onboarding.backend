using bit.api.Domain.Contracts;
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
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _commentRepository;
        private readonly IRepository<CommentLike> _commentLikeRepository;

        public CommentService(IRepository<Comment> commentRepository,
                                IRepository<CommentLike> commentLikeRepository)
        {
            _commentRepository = commentRepository;
            _commentLikeRepository = commentLikeRepository;
        }

        public async Task<CommentViewModel> CreateComment(string postId, string content, UserViewModel author)
        {
            try
            {
                // TODO: Consider refactor for Comment and CommentViewModel when interacting with CommentService.
                var comment = new Comment
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UId = Guid.NewGuid(),
                    Author = author,
                    Content = content,
                    PostId = postId.ToLower(),
                    CreatedOn = DateTime.UtcNow,
                };

                await _commentRepository.Add(comment);

                var commentViewModel = new CommentViewModel
                {
                    Id = comment.Id,
                    Author = comment.Author,
                    Content = comment.Content,
                    CreatedOn = DateTime.UtcNow,
                };

                return commentViewModel;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<CommentViewModel>> GetCommentsByPostId(string postId, int offset)
        {
            var comments = _commentRepository
                .Get(x => 
                    x.PostId == postId.ToLower() 
                    && !x.IsDeleted)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(offset)
                .Take(5)
                .Select(x => new CommentViewModel
                    {
                        Id = x.Id,
                        Author = x.Author,
                        Content = x.Content,
                        CreatedOn = x.CreatedOn,
                        UpdatedOn = x.UpdatedOn,
                        UpdatedBy = x.UpdatedBy
                    })
                .ToList();

            return comments;
        }

        public void GetReactionsForComments(string userId, List<CommentViewModel> comments)
        {
            try
            {
                comments.ForEach(x =>
                {
                    var reaction = _commentLikeRepository
                        .Get(y => y.CommentId == x.Id
                            && y.UserId == userId
                            && !y.IsDeleted)
                        .FirstOrDefault();

                    if (reaction is object)
                    {
                        x.isLiked = reaction.Type == CommentLikeType.Like;
                        x.isDisliked = reaction.Type == CommentLikeType.Dislike;
                    }
                });

                return;
            }
            catch (System.Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> LikeCommentById(string userId, string commentId)
        {
            try
            {
                var currentCommentLike = _commentLikeRepository
                    .Get(x =>
                        x.UserId == userId
                        && x.CommentId == commentId.ToLower() 
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (currentCommentLike is object)
                {
                    if (currentCommentLike.Type == CommentLikeType.Like)
                    {
                        return true;
                    } 
                    else
                    {
                        currentCommentLike.IsDeleted = true;

                        // TODO: can we batch these together anyway?
                        // TODO: the two database operations aren't atomic.
                        await _commentLikeRepository.Update(currentCommentLike);
                    }
                }

                var commentLike = new CommentLike
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UId = Guid.NewGuid(),
                    UserId = userId,
                    CommentId = commentId.ToLower(),
                    Type = CommentLikeType.Like,
                };

                await _commentLikeRepository.Add(commentLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UnlikeCommentById(string userId, string commentId)
        {
            try
            {
                var commentLike = _commentLikeRepository
                    .Get(x =>
                        x.UserId == userId
                        && x.CommentId == commentId.ToLower()
                        && x.Type == CommentLikeType.Like
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (commentLike is null)
                {
                    return true;
                }

                commentLike.IsDeleted = true;

                await _commentLikeRepository.Update(commentLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> DislikeCommentById(string userId, string commentId)
        {
            try
            {
                var currentCommentLike = _commentLikeRepository
                    .Get(x =>
                        x.UserId == userId
                        && x.CommentId == commentId.ToLower()
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (currentCommentLike is object)
                {
                    if (currentCommentLike.Type == CommentLikeType.Dislike)
                    {
                        return true;
                    }
                    else
                    {
                        currentCommentLike.IsDeleted = true;

                        //TODO: Batch these together?
                        await _commentLikeRepository.Update(currentCommentLike);
                    }
                }

                var commentLike = new CommentLike
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    UId = Guid.NewGuid(),
                    UserId = userId,
                    CommentId = commentId,
                    Type = CommentLikeType.Dislike,
                };

                await _commentLikeRepository.Add(commentLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UndislikeCommentById(string userId, string commentId)
        {
            try
            {
                var commentLike = _commentLikeRepository
                    .Get(x =>
                        x.UserId == userId
                        && x.CommentId == commentId.ToLower()
                        && x.Type == CommentLikeType.Dislike
                        && !x.IsDeleted)
                    .SingleOrDefault();

                if (commentLike is null)
                {
                    return true;
                }

                commentLike.IsDeleted = true;

                await _commentLikeRepository.Update(commentLike);

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
