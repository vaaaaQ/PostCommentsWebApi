using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PostComments.BLL.Entities.Comment;
using PostComments.BLL.ViewModels;

namespace PostComments.BLL.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(CreateCommentViewModel viewModel);
        Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId);
        Task<Comment> GetCommentById(Guid commentId);
        Task<Comment> UpdateCommentAsync(UpdateCommentViewModel viewModel);
        Task DeleteCommentAsync(Guid commentId);
    }
}