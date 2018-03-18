using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Exceptions;
using PostComments.BLL.Interfaces;
using PostComments.BLL.Specifications;
using PostComments.BLL.ViewModels;

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
        /// <param name="viewModel">comment data</param>
        /// <returns></returns>
        public async Task<Comment> CreateCommentAsync(CreateCommentViewModel viewModel)
        {
            Guard.Against.Null(viewModel, nameof(viewModel));
            Guard.Against.GuidEmpty(viewModel.FromId, nameof(viewModel.FromId));
            Guard.Against.GuidEmpty(viewModel.PostId, nameof(viewModel.PostId));
            Guard.Against.NullOrEmpty(viewModel.Text, nameof(viewModel.Text));


            //check post exists
            var post = await _postRepository.GetByIdAsync(viewModel.PostId);
            Guard.Against.PostNotExists(post, viewModel.PostId);

            //create post based on commentDto
            var comment = new Comment(viewModel.FromId, viewModel.PostId, new Content(viewModel.Text));
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
        /// <param name="viewModel">new comment data</param>
        /// <returns>updated comment</returns>
        public async Task<Comment> UpdateCommentAsync(UpdateCommentViewModel viewModel)
        {
            Guard.Against.Null(viewModel, nameof(viewModel));
            Guard.Against.GuidEmpty(viewModel.CommentId, nameof(viewModel.CommentId));

            //Check Comment exists
            var comment = await _commentRepository.GetByIdAsync(viewModel.CommentId);
            Guard.Against.CommentNotExists(comment, viewModel.CommentId);

            comment.Content.Text = viewModel.Text;

            await _commentRepository.UpdateAsync(comment);

            return await _commentRepository.GetByIdAsync(viewModel.CommentId);
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