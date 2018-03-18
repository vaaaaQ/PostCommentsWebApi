using System.ComponentModel.DataAnnotations;

namespace PostComments.Service.Dtos
{
    public class UpdateCommentDto
    {
        [Required]
        public string Text { get; set; }
    }
}