using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class AppUser : IdentityUser
{
    public string? ProfilPhoto { get; set; } = "default_user_photo.png";
    public int? TotalScore { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool? IsActive { get; set; }
    public int? SuccessfulGames { get; set; }
    public int? CompletedGames { get; set; }
    public string? Level { get; set; }
}
