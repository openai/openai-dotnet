using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Tests
{
    internal enum TestScenario
    {
        Assistants,
        Audio_TTS,
        Audio_Whisper,
        Audio_Gpt_4o_Mini_Transcribe,
        Batch,
        Chat,
        Embeddings,
        Files,
        FineTuning,
        Images,
        LegacyCompletions,
        Models,
        Moderations,
        Realtime,
        Responses,
        VectorStores,
        TopLevel,
    }
}
