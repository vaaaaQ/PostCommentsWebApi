using System.ComponentModel.DataAnnotations;

namespace PostComments.Service.Dtos
{
    public class CreateCommentDto
    {
        [Required]
        public string Text { get; set; }
    }
}