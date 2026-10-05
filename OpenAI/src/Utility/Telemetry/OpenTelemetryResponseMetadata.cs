using OpenAI.Responses;

namespace OpenAI.Telemetry;

internal struct OpenTelemetryResponseMetadata
{
    public string Id { get; private set; }
    public string Model { get; private set; }
    public string ServiceTier { get; private set; }
    public string SystemFingerprint { get; private set; }

    public void Update(ResponseResult response)
    {
        Id = response.Id ?? Id;
        Model = response.Model ?? Model;
        ServiceTier = response.ServiceTier?.ToString() ?? ServiceTier;
        SystemFingerprint = OpenTelemetryTokenUsage.GetResponseSystemFingerprint(response) ?? SystemFingerprint;
    }
}
