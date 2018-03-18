using System;

namespace PostComments.BLL.ViewModels
{
    public class UpdateCommentViewModel
    {
        public string Text { get; set; }
        public Guid CommentId { get; set; }
    }
}