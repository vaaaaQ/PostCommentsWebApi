using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using PostComments.Core.Entities.Post;
using PostComments.Core.Exceptions;
using PostComments.Core.Interfaces;

namespace PostComments.Core.Services
{
    public class CreatePostDTO
    {
        public string Text { get; set; }
        public string Title { get; set; }
    }

    public class PostService
    {
        private readonly IAsyncRepository<Post> _postRepository;


        public PostService(IAsyncRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task<Post> CreatePostAsync(CreatePostDTO createPostDto, Guid from)
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

        public Task<Post> GetPostByIdAsync(Guid id)
        {
            Guard.Against.GuidEmpty(id, nameof(id));
            return _postRepository.GetByIdAsync(id);
        }
    }
}
