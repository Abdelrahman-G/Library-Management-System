namespace Library_Management_System.Models;

public class Publisher
{
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = new List<Book>();
}
