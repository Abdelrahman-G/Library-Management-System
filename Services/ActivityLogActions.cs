namespace Library_Management_System.Services;

public static class ActivityLogActions
{
    public const string LoginSucceeded = "Auth.LoginSucceeded";
    public const string Logout = "Auth.Logout";

    public const string PublisherCreated = "Publisher.Created";
    public const string PublisherUpdated = "Publisher.Updated";
    public const string PublisherDeleted = "Publisher.Deleted";
    public const string AuthorCreated = "Author.Created";
    public const string AuthorUpdated = "Author.Updated";
    public const string AuthorDeleted = "Author.Deleted";
    public const string CategoryCreated = "Category.Created";
    public const string CategoryUpdated = "Category.Updated";
    public const string CategoryDeleted = "Category.Deleted";
    public const string BookCreated = "Book.Created";
    public const string BookUpdated = "Book.Updated";
    public const string BookDeleted = "Book.Deleted";
    public const string BookCopyCreated = "BookCopy.Created";
    public const string BookCopyUpdated = "BookCopy.Updated";
    public const string BookCopyDeleted = "BookCopy.Deleted";
    public const string MemberCreated = "Member.Created";
    public const string MemberUpdated = "Member.Updated";
    public const string MemberDeleted = "Member.Deleted";
    public const string RoleCreated = "Role.Created";
    public const string RoleUpdated = "Role.Updated";
    public const string RoleDeleted = "Role.Deleted";
    public const string SystemUserCreated = "SystemUser.Created";
    public const string SystemUserUpdated = "SystemUser.Updated";
    public const string SystemUserDeactivated = "SystemUser.Deactivated";
    public const string SystemUserRolesChanged = "SystemUser.RolesChanged";
    public const string SystemUserSessionsTerminated = "SystemUser.SessionsTerminated";
    public const string BorrowingCheckedOut = "Borrowing.CheckedOut";
    public const string BorrowingReturned = "Borrowing.Returned";
}
