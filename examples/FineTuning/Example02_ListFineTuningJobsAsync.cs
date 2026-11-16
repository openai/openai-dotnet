using NUnit.Framework;
using OpenAI.FineTuning;
using System;
using System.Threading.Tasks;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

public partial class FineTuningExamples
{
    [Test]
    public async Task Example02_ListFineTuningJobsAsync()
    {
        FineTuningClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        FineTuningJobCollectionOptions options = new() { PageSize = 10 };

        int count = 0;
        await foreach (FineTuningJob job in client.GetJobsAsync(options))
        {
            Console.WriteLine($"{job.JobId}: {job.Status} ({job.BaseModel})");
            if (++count == 10)
            {
                break;
            }
        }
    }
}

#pragma warning restore OPENAI001
