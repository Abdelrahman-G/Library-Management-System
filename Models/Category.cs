namespace Library_Management_System.Models;
public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }
    public ICollection<Category> ChildCategories { get; set; } = new List<Category>();

    public ICollection<BookCategory> BookCategories { get; set; } = new List<BookCategory>();
}
