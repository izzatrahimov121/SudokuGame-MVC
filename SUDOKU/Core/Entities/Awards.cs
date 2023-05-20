using Core.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Awards : IEntity
{
    public int Id { get; set; }


    [Required]
    public string? UserId { get; set; }
    public AppUser? AppUser { get; set; }



    [Required]
    public string? Month { get; set; }

    [Required]
    public int? Day { get; set; }

    [Required]
    public int? CompletedDays { get; set;}

    [Required]
    public string? Cup { get; set; }

}
