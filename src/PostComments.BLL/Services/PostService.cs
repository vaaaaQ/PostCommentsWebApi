using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.Exceptions;
using PostComments.BLL.Interfaces;
using PostComments.Core.Interfaces;

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
        /// <param name="createPostDto">post data</param>
        /// <param name="from">user id</param>
        /// <returns>Created post</returns>
        public async Task<Post> CreatePostAsync(CreatePostDto createPostDto, Guid from)
        {
            Guard.Against.Null(createPostDto, nameof(createPostDto));
            Guard.Against.NullOrEmpty(createPostDto.Text, nameof(createPostDto.Text));
            Guard.Against.NullOrEmpty(createPostDto.Title, nameof(createPostDto.Title));
            Guard.Against.GuidEmpty(from, nameof(from));

            var post = new Post(new Content(createPostDto.Text), new Title(createPostDto.Text), from);
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
        /// <param name="initialPostId">post id</param>
        /// <param name="postData">post data</param>
        /// <returns></returns>
        public async Task UpdatePostAsync(Guid initialPostId, UpdatePostDto postData)
        {
            Guard.Against.GuidEmpty(initialPostId, nameof(initialPostId));
            Guard.Against.Null(postData, nameof(postData));

            if (string.IsNullOrEmpty(postData.Text) && string.IsNullOrEmpty(postData.Title))
                throw new ArgumentNullException("At least 1 value shouldn't be empty");

            var post = await _postRepository.GetByIdAsync(initialPostId);

            Guard.Against.PostNotExists(post, initialPostId);

            post.Content.Text = postData.Text;
            post.Title.Text = postData.Title;

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