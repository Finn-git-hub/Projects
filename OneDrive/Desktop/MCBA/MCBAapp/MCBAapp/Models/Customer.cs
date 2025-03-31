
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAapp.Filters;

namespace MCBAapp.Models;

public partial class Customer
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "Must be 4 digits!")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Must be numeric!")]
    public int CustomerID { get; set; }

    [StringLength(50,MinimumLength = 1,ErrorMessage = "at least a character")]
    public string Name { get; set; }
    
    [StringLength(50)]
    public string? Address { get; set; }
    [StringLength(40)]
    public string? City { get; set; }
    
    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
    public string? Postcode { get; set; }
    
    [StringLength(11)]
    [RegularExpression(@"^\d{3} \d{3} \d{3}$", ErrorMessage = "TFN must be in the format XXX XXX XXX")]
    public string? TFN { get; set; }
    
    [StringLength(3)]
    [RegularExpression(@"^(NSW|VIC|QLD|SA|WA|TAS|NT|ACT)$", ErrorMessage = "State must be a 2 or 3 lettered Australian state")]
    public string? State { get; set; }
    
    [StringLength(12)]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$", ErrorMessage = "Mobile must be in the format 04XX XXX XXX")]
    public string? Mobile { get; set; }
    
    public bool IsLocked { get; set; } = false;
    
    public virtual ICollection<Account> Accounts { get; set; }
    
    [NotMapped]
    public virtual Login Login { get; set; }
}