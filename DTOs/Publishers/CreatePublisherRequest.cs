using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Publishers;

public class CreatePublisherRequest
{
    [Required]
    [StringLength(255, MinimumLength = 2)]
    public string PublisherName { get; set; } = string.Empty;
}
