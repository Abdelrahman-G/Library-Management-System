namespace Library_Management_System.DTOs.Members;

public class MemberResponse
{
    public int MemberId { get; set; }
    public string MembershipNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Address { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public int BorrowingCount { get; set; }
}

