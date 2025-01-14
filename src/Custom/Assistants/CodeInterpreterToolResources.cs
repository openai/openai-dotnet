using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.Assistants;

/// <summary> The AssistantObjectToolResourcesCodeInterpreter. </summary>
[Experimental("OPENAI001")]
[CodeGenModel("AssistantObjectToolResourcesCodeInterpreter")]
public partial class CodeInterpreterToolResources
{
    private ChangeTrackingList<string> _fileIds = new ChangeTrackingList<string>();

    /// <summary> A list of [file](/docs/api-reference/files) IDs made available to the `code_interpreter`` tool. There can be a maximum of 20 files associated with the tool. </summary>
    public IList<string> FileIds
    {
        get => _fileIds;
        set
        {
            _fileIds = new ChangeTrackingList<string>();
            foreach (string fileId in value)
            {
                _fileIds.Add(fileId);
            }
        }
    }

    public CodeInterpreterToolResources()
    { }
}
