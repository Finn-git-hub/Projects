using MCBAapp.Models.Enums;
using Newtonsoft.Json;

namespace MCBAapp.Converters;

public class TransactionTypeConverter : JsonConverter<TransactionType>
{
    public override void WriteJson(JsonWriter writer, TransactionType value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    } 

    public override TransactionType ReadJson(JsonReader reader, Type objectType, TransactionType existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        var type = (string)reader.Value;
        return type switch
        {
            "D" => TransactionType.Deposit,
            "W" => TransactionType.Withdraw,
            "T" => TransactionType.Transfer,
            "S" => TransactionType.ServiceCharge,
            _ => TransactionType.Deposit
        };
    }
}