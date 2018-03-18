using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Exceptions;
using PostComments.BLL.Interfaces;
using PostComments.BLL.ViewModels;

namespace PostComments.BLL.Services
{
    public class PostService : IPostService
    {
        private readonly IAsyncRepository<Post> _postRepository;

        public PostService(IAsyncRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }


        /// <summary>
        /// Creates new post
        /// </summary>
        /// <param name="viewModel">post data</param>
        /// <returns>Created post</returns>
        public async Task<Post> CreatePostAsync(CreatePostViewModel viewModel)
        {
            Guard.Against.Null(viewModel, nameof(viewModel));
            Guard.Against.NullOrEmpty(viewModel.Text, nameof(viewModel.Text));
            Guard.Against.NullOrEmpty(viewModel.Title, nameof(viewModel.Title));
            Guard.Against.GuidEmpty(viewModel.FromId, nameof(viewModel.FromId));

            var post = new Post(new Content(viewModel.Text), new Title(viewModel.Text), viewModel.FromId);
            await _postRepository.AddAsync(post);
            return await _postRepository.GetByIdAsync(post.Id);
        }

        /// <summary>
        /// Returns all posts
        /// </summary>
        /// <returns>all posts</returns>
        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _postRepository.ListAllAsync();
        }

        /// <summary>
        /// Returns posts
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Created post</returns>
        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            Guard.Against.GuidEmpty(id, nameof(id));

            var post = await _postRepository.GetByIdAsync(id);
            Guard.Against.PostNotExists(post, id);

            return post;
        }

        /// <summary>
        /// Update post
        /// </summary>
        /// <param name="viewModel">post data</param>
        /// <returns></returns>
        public async Task UpdatePostAsync(UpdatePostViewModel viewModel)
        {
            Guard.Against.GuidEmpty(viewModel.InitialPostId, nameof(viewModel.InitialPostId));
            Guard.Against.Null(viewModel, nameof(viewModel));

            if (string.IsNullOrEmpty(viewModel.Text) && string.IsNullOrEmpty(viewModel.Title))
                throw new ArgumentNullException("At least 1 value shouldn't be empty");

            var post = await _postRepository.GetByIdAsync(viewModel.InitialPostId);

            Guard.Against.PostNotExists(post, viewModel.InitialPostId);

            post.Content.Text = viewModel.Text;
            post.Title.Text = viewModel.Title;

            await _postRepository.UpdateAsync(post);
        }

        /// <summary>
        /// Delete post
        /// </summary>
        /// <param name="id">post id</param>
        /// <returns></returns>
        public async Task DeletePostByIdAsync(Guid id)
        {
            Guard.Against.GuidEmpty(id, nameof(id));

            var post = await _postRepository.GetByIdAsync(id);

            Guard.Against.PostNotExists(post, id);

            if (post is null)
                throw new PostNotExistsException(id);

            await _postRepository.DeleteAsync(post);
        }
    }
}