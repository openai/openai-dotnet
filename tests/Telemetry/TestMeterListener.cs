using NUnit.Framework;
using OpenAI.Chat;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace OpenAI.Tests.Telemetry;

internal class TestMeterListener : IDisposable
{
    public record TestMeasurement(object value, Dictionary<string, object> tags);

    private readonly ConcurrentDictionary<string, List<TestMeasurement>> _measurements = new();
    private readonly ConcurrentDictionary<string, Instrument> _instruments = new();
    private readonly MeterListener _listener;
    public TestMeterListener(string meterName)
    {
        _listener = new MeterListener();
        _listener.InstrumentPublished = (i, l) =>
        {
            if (i.Meter.Name == meterName)
            {
                l.EnableMeasurementEvents(i);
            }
        };
        _listener.SetMeasurementEventCallback<long>(OnMeasurementRecorded);
        _listener.SetMeasurementEventCallback<double>(OnMeasurementRecorded);
        _listener.Start();
    }

    public List<TestMeasurement> GetMeasurements(string instrumentName)
    {
        _measurements.TryGetValue(instrumentName, out var list);
        return list;
    }

    public Instrument GetInstrument(string instrumentName)
    {
        _instruments.TryGetValue(instrumentName, out var instrument);
        return instrument;
    }

    private void OnMeasurementRecorded<T>(Instrument instrument, T measurement, ReadOnlySpan<KeyValuePair<string, object>> tags, object state)
    {
        _instruments.TryAdd(instrument.Name, instrument);

        var testMeasurement = new TestMeasurement(measurement, new Dictionary<string, object>(tags.ToArray()));
        _measurements.AddOrUpdate(instrument.Name,
            k => new() { testMeasurement },
            (k, l) =>
            {
                l.Add(testMeasurement);
                return l;
            });
    }

    public void Dispose()
    {
        _listener.Dispose();
    }

    public static void ValidateChatMetricTags(TestMeasurement measurement, ChatCompletion response, string requestModel = "gpt-4o-mini", string host = "api.openai.com", int port = 443)
    {
        Assert.AreEqual("openai", measurement.tags["gen_ai.system"]);
        Assert.AreEqual("chat", measurement.tags["gen_ai.operation.name"]);
        Assert.AreEqual(host, measurement.tags["server.address"]);
        Assert.AreEqual(requestModel, measurement.tags["gen_ai.request.model"]);
        Assert.AreEqual(port, measurement.tags["server.port"]);

        if (response != null)
        {
            Assert.AreEqual(response.Model, measurement.tags["gen_ai.response.model"]);
            Assert.False(measurement.tags.ContainsKey("error.type"));
        }
    }

    public static void ValidateChatMetricTags(TestMeasurement measurement, Exception ex, string requestModel = "gpt-4o-mini", string host = "api.openai.com", int port = 443)
    {
        ValidateChatMetricTags(measurement, (ChatCompletion)null, requestModel, host, port);
        Assert.True(measurement.tags.ContainsKey("error.type"));
        Assert.AreEqual(ex.GetType().FullName, measurement.tags["error.type"]);
    }
}
