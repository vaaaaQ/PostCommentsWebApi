using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostComments.Core.Entities.Post;

namespace PostComments.Core.Services
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(CreatePostDto createPostDto, Guid from);
        Task DeletePostByIdAsync(Guid id);
        Task<Post> GetPostByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetPostsAsync();
        Task UpdatePostAsync(Guid initialPostId, UpdatePostDto dto);
    }
}