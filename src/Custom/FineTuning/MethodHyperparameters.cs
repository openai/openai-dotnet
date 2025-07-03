using System;
using System.ClientModel.Primitives;
using System.Diagnostics.CodeAnalysis;

namespace OpenAI.FineTuning;

[Experimental("OPENAI001")]
public partial class MethodHyperparameters
{
    private static readonly BinaryData Auto = new("\"auto\"");
    internal float HandleDefaults(BinaryData data)
    {
        if (data is null)
        {
            throw new ArgumentNullException("This hyperparameter is not set. Values for BatchSize and LearningRateMultiplier are not available in responses.");
        }

        if (data.ToString() == Auto.ToString())
        {
            return 0;
        }

        try
        {
            return float.Parse(data.ToString());
        }
        catch (FormatException)
        {
            throw new FormatException($"Hyperparameter expected a number or \"auto\" string. Got {data}");
        }
    }
}