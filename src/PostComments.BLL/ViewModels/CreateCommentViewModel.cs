using System;

namespace PostComments.BLL.ViewModels
{
    public class CreateCommentViewModel
    {
        public string Text;
        public Guid FromId { get; set; }
        public Guid PostId { get; set; }
    }
}