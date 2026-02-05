using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

[CodeGenType("CreateModerationRequestModel")]
internal readonly partial struct InternalCreateModerationRequestModel { }

[CodeGenType("CreateModerationResponseResultsCategories")]
internal partial class InternalModerationCategories { }

[CodeGenType("CreateModerationResponseResultsCategoryScores")]
internal partial class InternalModerationCategoryScores { }

[CodeGenType("ModerationImageURLInput")]
internal partial class InternalModerationImagePart { }

[CodeGenType("ModerationImageURLInputImageUrl")]
internal partial class InternalModerationImagePartImageUrl { }

[CodeGenType("UnknownCreateModerationRequestInput")]
internal partial class InternalUnknownModerationInputPart { }
