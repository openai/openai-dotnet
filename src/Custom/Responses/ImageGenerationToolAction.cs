using System.Diagnostics.CodeAnalysis;
using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Responses
{
    [Experimental("OPENAI001")]
    [CodeGenType("ImageGenerationToolAction")]
    public enum ImageGenerationToolAction
    {
        Generate,
        Edit,
        Auto
    }
}
