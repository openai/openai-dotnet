using Microsoft.TypeSpec.Generator.Customizations;
using OpenAI.Assistants;

namespace OpenAI;

[CodeGenSuppress("RunStepFileSearchResultContent", typeof(string))]
[CodeGenType("OpenAIModelFactory")]
internal static partial class OpenAIModelFactory
{
	public static RunStepFileSearchResultContent RunStepFileSearchResultContent(string text = default)
	{
		return new RunStepFileSearchResultContent(RunStepFileSearchResultContentKind.Text, text, additionalBinaryDataProperties: null);
	}
}
