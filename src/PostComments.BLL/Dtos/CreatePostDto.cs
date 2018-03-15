
using System.ComponentModel.DataAnnotations;

namespace PostComments.BLL.Dtos
{
    public class CreatePostDto
    {
        [Required]
        public string Text { get; set; }
        [Required]
        public string Title { get; set; }
    }
}