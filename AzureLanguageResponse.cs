using System.Text.Json.Serialization;

namespace EchoBot;

public class AzureLanguageResponse
{
    public AzureLanguageResponseResult Result { get; init; }
}

public class AzureLanguageResponseResult
{
    public AzureLanguageResponsePrediction Prediction { get; init; }
}

public class AzureLanguageResponsePrediction
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public Intent TopIntent { get; init; }
}

