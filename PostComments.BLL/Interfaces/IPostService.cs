using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostComments.BLL.Dtos;
using PostComments.BLL.Entities.Post;

namespace PostComments.Core.Interfaces
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