﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.Core.Entities.Comment;
using PostComments.Core.Entities.Post;
using PostComments.Core.Exceptions;
using PostComments.Core.Interfaces;
using PostComments.Core.Specifications;

namespace UnitTests.PostComments.Core.Services
{
    public class CommentService : ICommentService
    {
        private readonly IAsyncRepository<Comment> _commentRepository;
        private readonly IAsyncRepository<Post> _postRepository;

        public CommentService(IAsyncRepository<Comment> commentRepository, IAsyncRepository<Post> postRepository)
        {
            this._commentRepository = commentRepository;
            _postRepository = postRepository;
        }

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
            Comment comment = new Comment(fromId, postId, new Content(commentDto.Text));
            await _commentRepository.AddAsync(comment);
            return await _commentRepository.GetByIdAsync(comment.Id);
        }

        public async Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId)
        {
            Guard.Against.GuidEmpty(postId, nameof(postId));

            //Check post exists
            var post = await _postRepository.GetByIdAsync(postId);
            Guard.Against.PostNotExists(post, postId);

            return await _commentRepository.ListAsync(
                new BaseSpecification<Comment>(comment => comment.PostId == postId));
        }

        public async Task<Comment> GetCommentById(Guid commentId)
        {
            Guard.Against.GuidEmpty(commentId, nameof(commentId));

            return await _commentRepository.GetByIdAsync(commentId);
        }

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