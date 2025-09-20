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
        Assert.That(activity, Is.Not.Null);
        Assert.That(activity.DisplayName, Is.EqualTo($"chat {requestModel}"));
        Assert.That(activity.GetTagItem("gen_ai.operation.name"), Is.EqualTo("chat"));
        Assert.That(activity.GetTagItem("gen_ai.system"), Is.EqualTo("openai"));
        Assert.That(activity.GetTagItem("gen_ai.request.model"), Is.EqualTo(requestModel));

        Assert.That(activity.GetTagItem("server.address"), Is.EqualTo(host));
        Assert.That(activity.GetTagItem("server.port"), Is.EqualTo(port));

        if (response != null)
        {
            Assert.That(activity.GetTagItem("gen_ai.response.model"), Is.EqualTo(response.Model));
            Assert.That(activity.GetTagItem("gen_ai.response.id"), Is.EqualTo(response.Id));
            Assert.That(activity.GetTagItem("gen_ai.response.finish_reasons"), Is.EqualTo(new[] { response.FinishReason.ToString().ToLower() }));
            Assert.That(activity.GetTagItem("gen_ai.usage.output_tokens"), Is.EqualTo(response.Usage.OutputTokenCount));
            Assert.That(activity.GetTagItem("gen_ai.usage.input_tokens"), Is.EqualTo(response.Usage.InputTokenCount));
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Unset));
            Assert.That(activity.StatusDescription, Is.Null);
            Assert.That(activity.GetTagItem("error.type"), Is.Null);
        }
        else
        {
            Assert.That(activity.Status, Is.EqualTo(ActivityStatusCode.Error));
            Assert.That(activity.GetTagItem("error.type"), Is.Not.Null);
        }
    }

    public static void ValidateChatActivity(Activity activity, Exception ex, string requestModel = "gpt-4o-mini", string host = "api.openai.com", int port = 443)
    {
        ValidateChatActivity(activity, (ChatCompletion)null, requestModel, host, port);
        Assert.That(activity.GetTagItem("error.type"), Is.EqualTo(ex.GetType().FullName));
    }
}
