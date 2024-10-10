using System.ComponentModel.DataAnnotations;

namespace SimpleJwtAuth.Models
{
    public class Blogs
    {
        [Key]
        public int BlogId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string Author { get; set; } = string.Empty;

    }
}
