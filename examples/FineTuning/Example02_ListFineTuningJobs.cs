using NUnit.Framework;
using OpenAI.FineTuning;
using System;

namespace OpenAI.Examples;

#pragma warning disable OPENAI001

public partial class FineTuningExamples
{
    [Test]
    public void Example02_ListFineTuningJobs()
    {
        FineTuningClient client = new(Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        FineTuningJobCollectionOptions options = new() { PageSize = 10 };

        int count = 0;
        foreach (FineTuningJob job in client.GetJobs(options))
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
