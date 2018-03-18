using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostComments.BLL.Entities.Post;
using PostComments.BLL.ViewModels;

namespace PostComments.BLL.Interfaces
{
    public interface IPostService
    {
        Task<Post> CreatePostAsync(CreatePostViewModel viewModel);
        Task DeletePostByIdAsync(Guid id);
        Task<Post> GetPostByIdAsync(Guid id);
        Task<IEnumerable<Post>> GetPostsAsync();
        Task UpdatePostAsync(UpdatePostViewModel viewModel);
    }
}