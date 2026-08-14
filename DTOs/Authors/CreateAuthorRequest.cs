using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Authors;

public class CreateAuthorRequest
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string AuthorName { get; set; } = string.Empty;
}

