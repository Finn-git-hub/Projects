
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAapp.Filters;

namespace MCBAapp.Models;

public partial class Login
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(8, MinimumLength = 8, ErrorMessage = "Must be 8 digits!")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Must be numeric!")]
    public string LoginID { get; set; }
    
    [ForeignKey("Customer")]
    public int CustomerID { get; set; }

    [Required,StringLength(94)]
    public string PasswordHash { get; set; }

    public virtual Customer Customer { get; set; }
    
}