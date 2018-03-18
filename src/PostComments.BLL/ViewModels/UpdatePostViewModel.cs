using System;

namespace PostComments.BLL.ViewModels
{
    public class UpdatePostViewModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Guid InitialPostId { get; set; }
    }
}