using NUnit.Framework;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenAI.Tests.Utility
{
    internal class DumpPolicy : PipelinePolicy
    {
        public override void Process(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        {
            ProcessRequest(message);
            ProcessNext(message, pipeline, currentIndex);
            ProcessResponse(message);
        }

        public override async ValueTask ProcessAsync(PipelineMessage message, IReadOnlyList<PipelinePolicy> pipeline, int currentIndex)
        {
            ProcessRequest(message);
            await ProcessNextAsync(message, pipeline, currentIndex);
            ProcessResponse(message);
        }

        private void ProcessRequest(PipelineMessage message)
        {
            Console.WriteLine($"--- New request ---");
            IEnumerable<string> headerPairs = message?.Request?.Headers?.Select(header => $"{header.Key}={(header.Key.ToLower().Contains("auth") ? "***" : header.Value)}");
            string headers = string.Join(',', headerPairs);
            Console.WriteLine($"Headers: {headers}");
            Console.WriteLine($"{message?.Request?.Method} URI: {message?.Request?.Uri}");
            if (message.Request?.Content != null)
            {
                string contentType = "Unknown Content Type";
                if (message.Request.Headers?.TryGetValue("Content-Type", out contentType) == true
                    && contentType == "application/json")
                {
                    using MemoryStream stream = new();
                    message.Request.Content.WriteTo(stream, default);
                    stream.Position = 0;
                    using StreamReader reader = new(stream);
                    string requestDump = reader.ReadToEnd();
                    stream.Position = 0;
                    requestDump = Regex.Replace(requestDump, @"""data"":[\\w\\r\\n]*""[^""]*""", @"""data"":""...""");
                    Console.WriteLine(requestDump);
                }
                else
                {
                    string length = message.Request.Content.TryComputeLength(out long numberLength)
                        ? $"{numberLength} bytes"
                        : "unknown length";
                    Console.WriteLine($"<< Non-JSON content: {contentType} >> {length}");
                }
            }
        }

        private void ProcessResponse(PipelineMessage message)
        {
            if (message.BufferResponse)
            {
                Console.WriteLine("--- Begin response content ---");
                Console.WriteLine(message.Response.Content?.ToString());
                Console.WriteLine("--- End of response content ---");
            }
            else
            {
                Console.WriteLine("--- Response (unbuffered, content not rendered) ---");
            }
        }
    }
}
