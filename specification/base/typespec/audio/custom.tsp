import "./models.tsp";

using TypeSpec.OpenAPI;

namespace OpenAI;

union TranscriptionAudioResponseFormat {
  AudioResponseFormat,
}

union TranslationAudioResponseFormat {
  AudioResponseFormat,
}

@minValue(0)
@maxValue(1)
scalar ZeroOrOneFloat extends float32;
