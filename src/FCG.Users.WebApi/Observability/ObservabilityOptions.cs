using System.Diagnostics.CodeAnalysis;

namespace FCG.Users.WebApi.Observability;

[ExcludeFromCodeCoverage]
public class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; set; } = "FCG.Users";
    public string OtlpEndpoint { get; set; } = string.Empty;
    public string OtlpAuthHeader { get; set; } = string.Empty;
    public bool EnableOtlpExporter { get; set; } = false;
}
