﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Exceptions;
using PostComments.BLL.Interfaces;
using PostComments.BLL.Specifications;

namespace PostComments.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IAsyncRepository<Post> _postRepository;

        public CommentService(IAsyncRepository<Comment> commentRepository, IAsyncRepository<Post> postRepository)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
        }

        /// <summary>
        /// Create new comment
        /// </summary>
        /// <param name="commentDto">comment data</param>
        /// <param name="fromId">user id (who commented)</param>
        /// <param name="postId">post id</param>
        /// <returns></returns>
        public async Task<Comment> CreateCommentAsync(CreateCommentDto commentDto, Guid fromId, Guid postId)
        {
            Guard.Against.Null(commentDto, nameof(commentDto));
            Guard.Against.GuidEmpty(fromId, nameof(fromId));
            Guard.Against.GuidEmpty(postId, nameof(postId));
            Guard.Against.NullOrEmpty(commentDto.Text, nameof(commentDto.Text));


            //check post exists
            var post = await _postRepository.GetByIdAsync(postId);
            Guard.Against.PostNotExists(post, postId);

            //create post based on commentDto
            var comment = new Comment(fromId, postId, new Content(commentDto.Text));
            await _commentRepository.AddAsync(comment);
            return await _commentRepository.GetByIdAsync(comment.Id);
        }

        /// <summary>
        /// Returns comment for paricular post
        /// </summary>
        /// <param name="postId">post id</param>
        /// <returns>comments</returns>
        public async Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId)
        {
            Guard.Against.GuidEmpty(postId, nameof(postId));

            //Check post exists
            var post = await _postRepository.GetByIdAsync(postId);
            Guard.Against.PostNotExists(post, postId);

            return await _commentRepository.ListAsync(
                new BaseSpecification<Comment>(comment => comment.PostId == postId));
        }


        /// <summary>
        /// Returns comment data
        /// </summary>
        /// <param name="commentId">comment id</param>
        /// <returns>comment</returns>
        public async Task<Comment> GetCommentById(Guid commentId)
        {
            Guard.Against.GuidEmpty(commentId, nameof(commentId));

            var comment = await _commentRepository.GetByIdAsync(commentId); 
            Guard.Against.CommentNotExists(comment, commentId);
            return comment;
        }

        /// <summary>
        /// Updates comment data
        /// </summary>
        /// <param name="updateCommentDto">new comment data</param>
        /// <param name="commentId">comment id</param>
        /// <returns>updated comment</returns>
        public async Task<Comment> UpdateCommentAsync(UpdateCommentDto updateCommentDto, Guid commentId)
        {
            Guard.Against.Null(updateCommentDto, nameof(updateCommentDto));
            Guard.Against.GuidEmpty(commentId, nameof(commentId));

            //Check Comment exists
            var comment = await _commentRepository.GetByIdAsync(commentId);
            Guard.Against.CommentNotExists(comment, commentId);

            comment.Content.Text = updateCommentDto.Text;

            await _commentRepository.UpdateAsync(comment);

            return await _commentRepository.GetByIdAsync(commentId);
        }

        /// <summary>
        /// Delete comment
        /// </summary>
        /// <param name="commentId">comment id</param>
        /// <returns></returns>
        public async Task DeleteCommentAsync(Guid commentId)
        {
            Guard.Against.GuidEmpty(commentId, nameof(commentId));

            //Check Comment exists
            var comment = await _commentRepository.GetByIdAsync(commentId);
            Guard.Against.CommentNotExists(comment, commentId);

            await _commentRepository.DeleteAsync(comment);
        }
    }
}