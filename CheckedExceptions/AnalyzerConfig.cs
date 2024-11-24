namespace Sundstrom.CheckedExceptions;

using System.Text.Json.Serialization;

public partial class AnalyzerConfig
{
    [JsonPropertyName("ignoredExceptions")]
    public IEnumerable<string> IgnoredExceptions { get; set; } = new List<string>();

    [JsonPropertyName("informationalExceptions")]
    public IDictionary<string, ExceptionMode> InformationalExceptions { get; set; } = new Dictionary<string, ExceptionMode>();
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ExceptionMode
{
    Throw = 1,
    Propagation = 2,
    Always = Throw | Propagation
}