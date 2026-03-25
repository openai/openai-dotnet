import "./common.shared.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http";

using Azure.ClientGenerator.Core;
using TypeSpec.Http;

namespace OpenAI;

// Here, we create scenario-specific copies of types for the .NET SDK.

// ResponseFormat

@access(Access.public)
@usage(Usage.input | Usage.output)
@discriminator("type")
model DotNetChatResponseFormat {
  ...ResponseFormat;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetChatResponseFormatText extends DotNetChatResponseFormat {
  ...ResponseFormatText;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetChatResponseFormatJsonObject extends DotNetChatResponseFormat {
  ...ResponseFormatJsonObject;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetChatResponseFormatJsonSchema extends DotNetChatResponseFormat {
  type: "json_schema";
  json_schema: {
    description?: string;
    name: string;
    schema?: unknown;
    strict?: boolean | null = false;
  };
}

// DotNetAssistantResponseFormat

@access(Access.public)
@usage(Usage.input | Usage.output)
@discriminator("type")
model DotNetAssistantResponseFormat {
  ...ResponseFormat;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatText extends DotNetAssistantResponseFormat {
  ...ResponseFormatText;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatJsonObject
  extends DotNetAssistantResponseFormat {
  ...ResponseFormatJsonObject;
}

@access(Access.public)
@usage(Usage.input | Usage.output)
model DotNetAssistantResponseFormatJsonSchema
  extends DotNetAssistantResponseFormat {
  type: "json_schema";
  json_schema: {
    description?: string;
    name: string;
    schema?: unknown;
    strict?: boolean | null = false;
  };
}

// LogProbProperties

@access(Access.public)
@usage(Usage.output)
model DotNetAudioLogProbsProperties {
  ...LogProbProperties;
}

@access(Access.public)
@usage(Usage.output)
model DotNetRealtimeLogProbsProperties {
  ...LogProbProperties;
}

// VoiceIdsShared

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetAudioVoiceIds {
  VoiceIdsShared,
}

@access(Access.public)
@usage(Usage.input | Usage.output)
union DotNetChatVoiceIds {
  VoiceIdsShared,
}

union DotNetChatServiceTier {
   ServiceTier
}


