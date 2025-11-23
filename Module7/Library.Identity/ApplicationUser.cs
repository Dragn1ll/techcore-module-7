using Microsoft.AspNetCore.Identity;

namespace Library.Identity;

public sealed class ApplicationUser : IdentityUser
{
    public DateOnly DateOfBirth { get; set; }
}