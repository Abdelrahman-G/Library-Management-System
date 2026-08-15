namespace Library_Management_System.Authorization;

public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string Librarian = "Librarian";
    public const string Staff = "Staff";
    public const string AdministratorOrLibrarian = Administrator + "," + Librarian;
    public const string CirculationRoles =
        Administrator + "," + Librarian + "," + Staff;
}
