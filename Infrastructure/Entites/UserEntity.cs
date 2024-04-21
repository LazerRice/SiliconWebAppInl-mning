using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entites;

public class UserEntity : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string ProfileImage { get; set; } = "Avatar.jpg";

    public string Bio { get; set; }

    public int? AddressId { get; set; }
    public AddressEntity Address { get; set; }
}
