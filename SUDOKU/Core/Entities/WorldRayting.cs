using Core.Interfaces;

namespace Core.Entities;

public class WorldRayting:IEntity
{
    public int Id { get; set; }
    public int Rayting { get; set; } = 0;
	public string UserId { get; set; }
	public AppUser? User { get; set; }
    ICollection<AppUser> Users { get; set; }
    public string UserName { get; set; }
    public string Photo { get; set; }
    public int ThreeStar { get; set; }
    public int TotalScore { get; set; }
}
