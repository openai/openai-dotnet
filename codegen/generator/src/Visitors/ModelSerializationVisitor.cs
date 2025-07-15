using Microsoft.TypeSpec.Generator.ClientModel;
using Microsoft.TypeSpec.Generator.ClientModel.Providers;
using Microsoft.TypeSpec.Generator.Input;
using Microsoft.TypeSpec.Generator.Providers;
using System.Linq;

namespace OpenAILibraryPlugin.Visitors;

/// <summary>
/// A visitor to add MRW serialization to models.
/// </summary>
public class ModelSerializationVisitor : ScmLibraryVisitor
{
    protected override ModelProvider? PreVisitModel(InputModelType model, ModelProvider? type)
    {
        if (type is null || model.Usage.HasFlag(InputModelTypeUsage.Json))
        {
            return base.PreVisitModel(model, type);
        }

        foreach (var provider in type.SerializationProviders)
        {
            if (provider is MrwSerializationTypeDefinition)
            {
                return base.PreVisitModel(model, type);
            }
        }

        var serializations = type.SerializationProviders.ToList();
        serializations.Add(new MrwSerializationTypeDefinition(model, type));
        type.Update(serializations: serializations);

        return base.PreVisitModel(model, type);
    }
}
