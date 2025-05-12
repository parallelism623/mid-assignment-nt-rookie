
using System.Text.Json;
using System.Text;

namespace MIDASM.Domain.Infrastructure.MessageBrokers;

public class Message<T>
{
    public MetaData MetaData { get; set; } = default;

    public T Data { get; set; } = default;

    public string SerializeObject()
    {
        return JsonSerializer.Serialize(this);
    }

    public byte[] GetBytes()
    {
        return Encoding.UTF8.GetBytes(SerializeObject());
    }
}