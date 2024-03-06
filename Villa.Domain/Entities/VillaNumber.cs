using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Villa.Domain.Entities;

public class VillaNumber
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Villa Number")]
    public int Villa_Number { get; set; }

    [ForeignKey("Villa")] 
    [Display(Name = "Villa ID")]
    public int VillaId { get; set; }
    [ValidateNever]
    public Villa Villa { get; set; }

    [Display(Name = "Special Details")]
    public string? SpecialDetails { get; set; }
}