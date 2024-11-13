using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace OpenAI.Assistants;

/// <summary>
/// Represents additional options available when creating a new <see cref="Assistant"/>.
/// </summary>
[Experimental("OPENAI001")]
[CodeGenModel("CreateThreadRequest")]
public partial class ThreadCreationOptions
{
    // CUSTOM: reuse a common type for request/response model representations of tool resources

    /// <inheritdoc cref="ToolResources"/>
    [CodeGenMember("ToolResources")]
    public ToolResources ToolResources { get; set; }

    // CUSTOM: the wire-oriented messages type list is hidden so that we can propagate top-level required semantics
    //          of message creation into the collection.

    [CodeGenMember("Messages")]
    internal IList<MessageCreationOptions> InternalMessages
    {
        get => InitialMessages.Select(initializationMessage => initializationMessage as MessageCreationOptions).ToList();
        private set
        {
            // Note: this path is exclusively used in a test or deserialization case; here, we'll convert the
            //          underlying wire-friendly representation into the initialization message abstraction.

            InitialMessages.Clear();
            foreach (MessageCreationOptions baseMessageOptions in value)
            {
                InitialMessages.Add(new ThreadInitializationMessage(baseMessageOptions));
            }
        }
    }

    /// <summary>
    /// The collection of new message definitions that should be added to the thread immediately upon its creation.
    /// </summary>
    /// <remarks>
    /// Items may be inserted into this collection via list initializer, e.g.:
    /// <para><code>
    /// <see cref="MessageCreationOptions"/> options = new()
    /// {
    ///     InitialMessages = { new <see cref="ThreadInitializationMessage"/>(["Hello, world!"]) },
    /// }
    /// </code></para>
    /// </remarks>
    public IList<ThreadInitializationMessage> InitialMessages { get; } = new ChangeTrackingList<ThreadInitializationMessage>();
}
