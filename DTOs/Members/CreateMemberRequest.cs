using System.ComponentModel.DataAnnotations;

namespace Library_Management_System.DTOs.Members;

public class CreateMemberRequest
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string MembershipNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string PhoneNumber { get; set; } = string.Empty;

    public DateTime DateOfBirth { get; set; }

    [Required]
    [StringLength(500)]
    public string Address { get; set; } = string.Empty;

    public DateTime? JoinDate { get; set; }
}

