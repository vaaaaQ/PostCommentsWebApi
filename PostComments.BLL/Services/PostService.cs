using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.Core.Entities.Post;
using PostComments.Core.Exceptions;
using PostComments.Core.Interfaces;

namespace PostComments.Core.Services
{
    public class PostService
    {
        private readonly IAsyncRepository<Post> _postRepository;


        public PostService(IAsyncRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post> CreatePostAsync(CreatePostDto createPostDto, Guid from)
        {
            Guard.Against.Null(createPostDto, nameof(createPostDto));
            Guard.Against.NullOrEmpty(createPostDto.Text, nameof(createPostDto.Text));
            Guard.Against.NullOrEmpty(createPostDto.Title, nameof(createPostDto.Title));
            Guard.Against.GuidEmpty(from, nameof(from));

            var post = new Post(new Content(createPostDto.Text), new Title(createPostDto.Text), from);
            return await _postRepository.AddAsync(post);
        }

        public async Task<IEnumerable<Post>> GetPostsAsync()
        {
            return await _postRepository.ListAllAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid id)
        {
            Guard.Against.GuidEmpty(id, nameof(id));
            var post = await _postRepository.GetByIdAsync(id);
            if (post is null)
                throw new PostNotExistsException(id);
            return post;
        }

        public async Task UpdatePostAsync(Guid initialPostId, UpdatePostDto dto)
        {
            Guard.Against.GuidEmpty(initialPostId, nameof(initialPostId));
            Guard.Against.Null(dto, nameof(dto));

            if (string.IsNullOrEmpty(dto.Text) && string.IsNullOrEmpty(dto.Title))
                throw new ArgumentNullException("At least 1 value shouldn't be empty");

            var post = await _postRepository.GetByIdAsync(initialPostId);

            if (post is null)
                throw new PostNotExistsException(initialPostId);

            post.Content.Text = dto.Text;
            post.Title.Text = dto.Title;

            await _postRepository.UpdateAsync(post);
        }

        public async Task DeletePostByIdAsync(Guid id)
        {
            Guard.Against.GuidEmpty(id, nameof(id));

            var post = await _postRepository.GetByIdAsync(id);

            if (post is null)
                throw new PostNotExistsException(id);

            await _postRepository.DeleteAsync(post);
        }
    }
}
