using System.CommandLine;
using OpenAI.SpecProcessor.Spec;

namespace OpenAI.SpecProcessor.Commands;

/// <summary> The 'identity' command: prints the processing identity of this build. </summary>
/// <remarks>
/// The orchestrator has to decide whether a run would produce the same snapshot as last time, and
/// that depends on the tool's behavior as much as on the source bytes. Rather than teach a
/// PowerShell script to recompute a hash over the feature map, the tool answers for itself. There is
/// one implementation of what the identity is, which is the only way the two can be guaranteed to
/// agree.
/// </remarks>
public static class IdentityCommand
{
    /// <summary> Creates the identity command. </summary>
    /// <returns> The configured <see cref="Command"/> instance. </returns>
    public static Command Create()
    {
        var fingerprintOnlyOpt = new Option<bool>("--fingerprint-only")
        {
            Description = "Print only the combined fingerprint, for scripted comparison"
        };

        var cmd = new Command("identity", "Print the processing identity: the behavior that would produce a snapshot");
        cmd.Options.Add(fingerprintOnlyOpt);

        cmd.SetAction(parseResult =>
        {
            Console.WriteLine(parseResult.GetValue(fingerprintOnlyOpt)
                ? ProcessingIdentity.Current.Fingerprint
                : ProcessingIdentity.Current.ToJson());

            return 0;
        });

        return cmd;
    }
}
