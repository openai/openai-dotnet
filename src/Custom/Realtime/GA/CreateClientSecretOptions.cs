using Microsoft.TypeSpec.Generator.Customizations;
using System.ClientModel;

namespace OpenAI.Realtime;

// CUSTOM: Renamed.
[CodeGenType("RealtimeCreateClientSecretRequestGA")]
public partial class GACreateClientSecretOptions
{
    // CUSTOM: Renamed.
    [CodeGenMember("ExpiresAfter")]
    public GARealtimeClientSecretExpirationPolicy ExpirationPolicy { get; set; }

    // CUSTOM: Renamed.
    [CodeGenMember("Session")]
    public GARealtimeSessionOptions SessionOptions { get; set; }

    public static implicit operator BinaryContent(GACreateClientSecretOptions createClientSecretOptions)
    {
        if (createClientSecretOptions == null)
        {
            return null;
        }
        return BinaryContent.Create(createClientSecretOptions, ModelSerializationExtensions.WireOptions);
    }
}
