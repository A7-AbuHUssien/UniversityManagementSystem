using Microsoft.AspNetCore.Identity;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.Entities;

public class UserRefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; }
    public Guid UserId { get; set; }
    public virtual IdentityUser<Guid> User { get; set; } = null!;
}