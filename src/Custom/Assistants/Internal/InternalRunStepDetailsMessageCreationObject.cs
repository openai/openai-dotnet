namespace OpenAI.Assistants;

[CodeGenType("RunStepDetailsMessageCreationObject")]
public partial class InternalRunStepDetailsMessageCreationObject : RunStepDetails
{
    /// <inheritdoc cref="InternalRunStepDetailsMessageCreationObjectMessageCreation.MessageId"/>
    public string InternalMessageId => _messageCreation.MessageId;

    [CodeGenMember("MessageCreation")]
    internal readonly InternalRunStepDetailsMessageCreationObjectMessageCreation _messageCreation;
}