using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using TankR.Data.Enums;
using TankR.Data.Models.Identity;

namespace TankR.Data.Models;


[Index(nameof(Email), IsUnique = true)]
[Index(nameof(PhoneNumber), IsUnique = true)]
public class User
{
    [Key]
    public int Id { get; set; }
    
    public string? IdentityUserId { get; set; }   // link to Identity
    public ApplicationUser IdentityUser { get; set; }
    
    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    public string? AvatarUrl { get; set; }
    
    
    public UserAddress Address { get; set; }
    
    [Required]
    public UserRole Role { get; set; }

    public int LoyaltyPoints { get; set; } 
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
