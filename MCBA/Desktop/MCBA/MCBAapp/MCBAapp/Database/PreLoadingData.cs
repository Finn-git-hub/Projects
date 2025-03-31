using Newtonsoft.Json;
using MCBAapp.Models;
using MCBAapp.Models.Enums;

namespace MCBAapp.Database;

public class PreLoadingData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<McbaContext>();
        
        if (context.Customers.Any()) 
            return;

        const string Url = "https://coreteaching01.csit.rmit.edu.au/~e103884/wdt/services/customers/";

        using var client = new HttpClient();
        var json = client.GetStringAsync(Url).Result;

        ICollection<Customer>? customers = JsonConvert.DeserializeObject<ICollection<Customer>>(json, new JsonSerializerSettings()
        {
            DateFormatString = "dd/MM/yyyy hh:mm:ss tt"
        });

        foreach (var customer in customers)
        {
            foreach (var account in customer.Accounts)
            {
                account.Balance = 0;
                foreach (var transaction in account.Transactions)
                {
                    account.Balance += transaction.Amount;
                    transaction.TransactionType = TransactionType.Deposit;
                }
            }
        }
        
        context.AddRange(customers);
        context.SaveChanges();
    }
    
    
}