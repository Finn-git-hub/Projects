
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAapp.Models.Enums;

namespace MCBAapp.Models;

public partial class BillPay
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BillPayID { get; set; }
    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    [ForeignKey("Payee")]
    public int PayeeID { get; set; }

    [DataType(DataType.Currency)]
    [Required, Column(TypeName = "money")]
    public decimal Amount { get; set; }
    
    [Column(TypeName = "datetime2")]
    [Required]
    public DateTime ScheduleTimeUtc { get; set; }
    
    [Required]
    public BillpayType BillType { get; set; }

    public virtual Account Account { get; set; }

    public virtual Payee Payee { get; set; }
}