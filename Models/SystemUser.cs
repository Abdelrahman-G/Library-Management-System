using Library_Management_System.Models;

public class SystemUser
{
    public int SystemUserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<SystemUserRole> SystemUserRoles { get; set; }
        = new List<SystemUserRole>();

    public ICollection<UserActivityLog> ActivityLogs { get; set; }
        = new List<UserActivityLog>();

    public ICollection<BorrowingTransaction> IssuedTransactions { get; set; }
        = new List<BorrowingTransaction>();

    public ICollection<BorrowingTransaction> ReceivedTransactions { get; set; }
        = new List<BorrowingTransaction>();
}
