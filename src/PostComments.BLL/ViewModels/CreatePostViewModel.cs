using System;

namespace PostComments.BLL.ViewModels
{
    public class CreatePostViewModel
    {
        public string Text { get; set; }
        public string Title { get; set; }
        public Guid FromId { get; set; }
    }
}