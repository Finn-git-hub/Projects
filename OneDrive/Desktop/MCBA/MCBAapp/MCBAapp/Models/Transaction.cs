
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MCBAapp.Converters;
using MCBAapp.Models.Enums;
using Newtonsoft.Json;


namespace MCBAapp.Models;


public partial class Transaction
{
    [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TransactionID { get; set; }
    
    [Required]
    [JsonConverter(typeof(TransactionTypeConverter))]
    public TransactionType TransactionType { get; set; }
    
    [ForeignKey("Account")]
    public int AccountNumber { get; set; }
    // this is the account number of the destination account
    // which is must be 4 digits
    [Display(Name = "Destination Account Number")]
    [StringLength(4, MinimumLength = 4, ErrorMessage = "Must be 4 digits!")]
    [RegularExpression(@"^\d+$", ErrorMessage = "Must be numeric!")]
    public int? DestinationAccountNumber { get; set; }
    
    [DataType(DataType.Currency)]
    [Required, Column(TypeName = "money")]
    public decimal Amount { get; set; }
    
    [StringLength(30, ErrorMessage = "Comment must be less than 30 characters!")]
    public string? Comment { get; set; }
    
    [Column(TypeName = "datetime2")]
    [Required]
    public DateTime TransactionTimeUtc { get; set; }

    public virtual Account Account { get; set; }


}