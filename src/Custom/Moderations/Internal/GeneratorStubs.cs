using Microsoft.TypeSpec.Generator.Customizations;

namespace OpenAI.Moderations;

[CodeGenType("CreateModerationRequestModel")]
internal readonly partial struct InternalCreateModerationRequestModel { }

[CodeGenType("CreateModerationResponseResultsCategories")]
internal partial class InternalModerationCategories { }

[CodeGenType("CreateModerationResponseResultsCategoryScores")]
internal partial class InternalModerationCategoryScores { }

[CodeGenType("CreateModerationRequestInputType")]
internal readonly partial struct InternalModerationInputPartType { }

[CodeGenType("ModerationImageURLInput")]
internal partial class InternalModerationImagePart { }

[CodeGenType("UnknownCreateModerationRequestInput")]
internal partial class InternalUnknownModerationInputPart { }
