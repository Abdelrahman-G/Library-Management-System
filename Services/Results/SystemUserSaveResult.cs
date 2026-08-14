using Library_Management_System.DTOs.SystemUsers;

namespace Library_Management_System.Services.Results;

public record SystemUserSaveResult(
    SystemUserSaveStatus Status,
    SystemUserResponse? User = null);
