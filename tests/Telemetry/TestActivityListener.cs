// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenAI.Tests.Telemetry;

internal class TestActivityListener : IDisposable
{
    private readonly ActivityListener _listener;
    private readonly ConcurrentQueue<Activity> stoppedActivities = new ConcurrentQueue<Activity>();

    public TestActivityListener(string sourceName)
    {
        _listener = new ActivityListener()
        {
            ActivityStopped = stoppedActivities.Enqueue,
            ShouldListenTo = s => s.Name == sourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };

        ActivitySource.AddActivityListener(_listener);
    }

    public List<Activity> Activities => stoppedActivities.ToList();

    public void Dispose()
    {
        _listener.Dispose();
    }

    public static void ValidateChatActivity(Activity activity, ChatCompletion response, string requestModel = "gpt-4o-mini", string host = "api.openai.com", int port = 443)
    {
        Assert.NotNull(activity);
        Assert.AreEqual($"chat {requestModel}", activity.DisplayName);
        Assert.AreEqual("chat", activity.GetTagItem("gen_ai.operation.name"));
        Assert.AreEqual("openai", activity.GetTagItem("gen_ai.system"));
        Assert.AreEqual(requestModel, activity.GetTagItem("gen_ai.request.model"));

        Assert.AreEqual(host, activity.GetTagItem("server.address"));
        Assert.AreEqual(port, activity.GetTagItem("server.port"));

        if (response != null)
        {
            Assert.AreEqual(response.Model, activity.GetTagItem("gen_ai.response.model"));
            Assert.AreEqual(response.Id, activity.GetTagItem("gen_ai.response.id"));
            Assert.AreEqual(new[] { response.FinishReason.ToString().ToLower() }, activity.GetTagItem("gen_ai.response.finish_reasons"));
            Assert.AreEqual(response.Usage.OutputTokens, activity.GetTagItem("gen_ai.usage.output_tokens"));
            Assert.AreEqual(response.Usage.InputTokens, activity.GetTagItem("gen_ai.usage.input_tokens"));
            Assert.AreEqual(ActivityStatusCode.Unset, activity.Status);
            Assert.Null(activity.StatusDescription);
            Assert.Null(activity.GetTagItem("error.type"));
        }
        else
        {
            Assert.AreEqual(ActivityStatusCode.Error, activity.Status);
            Assert.NotNull(activity.GetTagItem("error.type"));
        }
    }

    public static void ValidateChatActivity(Activity activity, Exception ex, string requestModel = "gpt-4o-mini", string host = "api.openai.com", int port = 443)
    {
        ValidateChatActivity(activity, (ChatCompletion)null, requestModel, host, port);
        Assert.AreEqual(ex.GetType().FullName, activity.GetTagItem("error.type"));
    }
}
