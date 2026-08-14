using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Categories;

public class CreateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
}

