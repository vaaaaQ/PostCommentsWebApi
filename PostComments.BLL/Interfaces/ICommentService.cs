using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PostComments.Core.Entities.Comment;
using UnitTests.PostComments.Core.Services;

namespace PostComments.Core.Interfaces
{
    public interface ICommentService
    {
        Task<Comment> CreateCommentAsync(CreateCommentDto commentDto, Guid from, Guid postId);
        Task<IEnumerable<Comment>> GetCommentsByPostId(Guid postId);
        Task<Comment> GetCommentById(Guid commentId);
        Task<Comment> UpdateCommentAsync(UpdateCommentDto updateCommentDto, Guid commentId);
        Task DeleteCommentAsync(Guid commentId);
    }
}
