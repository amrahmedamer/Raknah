using Microsoft.AspNetCore.Identity;

namespace Raknah.Entity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public ICollection<Reservation> Reservations { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
