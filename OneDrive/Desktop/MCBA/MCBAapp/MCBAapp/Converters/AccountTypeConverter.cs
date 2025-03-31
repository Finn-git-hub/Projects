using MCBAapp.Models;
using MCBAapp.Models.Enums;
using Newtonsoft.Json;


namespace MCBAapp.Converters;

public class AccountTypeConverter : JsonConverter<AccountType>
{
   public override void WriteJson(JsonWriter writer, AccountType value, JsonSerializer serializer)
   {
      throw new NotImplementedException();
   }

   public override AccountType ReadJson(JsonReader reader, Type objectType, AccountType existingValue, bool hasExistingValue,
      JsonSerializer serializer)
   {
      var type = (string)reader.Value;

      return type switch
      {
         "S" => AccountType.Savings,
         "C" => AccountType.Checking,
         _ => throw new InvalidOperationException($"Unknown Account type: {type}")
      };

   }
}

