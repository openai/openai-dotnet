using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeCreateClientSecretRequestGA")]
public partial class CreateClientSecretOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("ExpiresAfter")]
    public RealtimeClientSecretExpirationPolicy ExpirationPolicy { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Session")]
    public RealtimeSessionOptions SessionOptions { get; set; }

    public static implicit operator BinaryContent(CreateClientSecretOptions createClientSecretOptions)
    {
        if (createClientSecretOptions == null)
        {
            return null;
        }
        return BinaryContent.Create(createClientSecretOptions, ModelSerializationExtensions.WireOptions);
    }
}
