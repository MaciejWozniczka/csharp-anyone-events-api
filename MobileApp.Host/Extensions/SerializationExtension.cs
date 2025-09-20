using Newtonsoft.Json;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace AnyOneApi.Host.Extensions;

public class SerializationExtension
{
    public static string SerializeWithoutNulls<T>(T model)
    {
        return JsonConvert.SerializeObject(model,
            Formatting.None,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
    }
}