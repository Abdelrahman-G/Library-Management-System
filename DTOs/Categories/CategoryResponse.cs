namespace Library_Management_System.DTOs.Categories;

public class CategoryResponse
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int ChildCategoryCount { get; set; }
    public int BookCount { get; set; }
}

