using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using MCBAapp.Converters;
using MCBAapp.Filters;
using MCBAapp.Models.Enums;

namespace MCBAapp.Models;

public partial class Account
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
    [Display(Name = "Account Number")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "Must be 4 digits!")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Must be numeric!")]
    public int AccountNumber { get; set; }
    
    [JsonConverter(typeof(AccountTypeConverter))]
    [Display(Name = "Type")]
    [Required]
    public AccountType AccountType { get; set; }

    [ForeignKey ("Customer")]
    [Required]
    public int CustomerID { get; set; }
    
    [DataType(DataType.Currency)]
    [Required, Column(TypeName = "money")]
    public decimal Balance { get; set; }

    public virtual Customer Customer { get; set; }

    public virtual ICollection<BillPay> BillPays { get; set; }
    
    [InverseProperty("Account")]
    public virtual ICollection<Transaction> Transactions { get; set; }
}