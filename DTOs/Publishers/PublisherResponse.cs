namespace Library_Management_System.DTOs.Publishers;

public class PublisherResponse
{
    public int PublisherId { get; set; }
    public string PublisherName { get; set; } = string.Empty;
    public int BookCount { get; set; }
}
