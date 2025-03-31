
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MCBAapp.Models;

public partial class Payee
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PayeeID { get; set; }
    
    [StringLength(50)]
    public string Name { get; set; }
    
    [StringLength(50)]
    public string Address { get; set; }
    
    [StringLength(40)]
    public string City { get; set; }

    [StringLength(3)]
    [RegularExpression(@"^(NSW|VIC|QLD|SA|WA|TAS|NT|ACT)$", ErrorMessage = "State must be a 2 or 3 lettered Australian state")]
    public string State { get; set; }

    [StringLength(4)]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Postcode must be 4 digits")]
    public string Postcode { get; set; }
    
    [StringLength(14)]
    [RegularExpression(@"^04\d{2} \d{3} \d{3}$", ErrorMessage = "Mobile must be in the format 04XX XXX XXX")]
    public string Phone { get; set; }

    public virtual ICollection<BillPay> BillPays { get; set; }

}