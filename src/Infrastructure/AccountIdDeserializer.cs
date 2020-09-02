namespace CleanDDDArchitecture.Infrastructure
{
    using System;
    using System.Text;
    using Application.Accounts;
    using Confluent.Kafka;

    public class AccountIdDeserializer : IDeserializer<AccountId>
    {
        public AccountId Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var decodedData = Encoding.UTF8.GetString(data.ToArray());
            
            return new AccountId(int.Parse(decodedData));
        }
    }
}