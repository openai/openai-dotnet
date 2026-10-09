using OpenAI.Responses;
using System;
using System.ClientModel.Primitives;
using System.Collections.Generic;
using System.Diagnostics;

namespace OpenAI.Telemetry;

internal partial class OpenTelemetryScope
{
    internal SseLifecycle<StreamingResponseUpdate> CreateStreamingLifecycle(Func<double> elapsedSeconds = null)
        => new ResponsesStreamingLifecycle(this, elapsedSeconds);

    private sealed class ResponsesStreamingLifecycle : SseLifecycle<StreamingResponseUpdate>
    {
        private readonly OpenTelemetryScope _scope;
        private readonly HashSet<int> _outputItems = [];
        private readonly Func<double> _elapsedSeconds;
        private bool _completed;
        private bool _receivedEvent;
        private bool _expectsTerminalResponse;
        private double _eventTime;
        private double? _previousOutputTime;
        private double? _firstChunkTime;
        private PipelineResponse _response;
        private OpenTelemetryResponseStream _responseStream;

        public ResponsesStreamingLifecycle(OpenTelemetryScope scope, Func<double> elapsedSeconds)
        {
            _scope = scope;
            _elapsedSeconds = elapsedSeconds;
        }

        public override IDisposable Enter() => new ActivityActivation(_completed ? null : _scope._activity);

        public override void OnResponse(PipelineResponse response)
        {
            if ((_scope._useLatestSemanticConventions) && (response.ContentStream is { } stream))
            {
                // Typed and raw enumeration share the send path; typed consumption detaches this observer.
                _response = response;
                _responseStream = new OpenTelemetryResponseStream(stream, Enter, CompleteRawResponse);
                response.ContentStream = _responseStream;
            }
        }

        public override void OnTypedResponse()
        {
            if (!_completed)
            {
                _expectsTerminalResponse = true;

                if (_responseStream is not null)
                {
                    _responseStream.Detach();
                    _response.ContentStream = _responseStream.InnerStream;
                    _responseStream = null;
                    _response = null;
                }
            }
        }

        private void CompleteRawResponse(Exception exception)
        {
            if (!_completed)
            {
                _completed = true;

                if (exception is not null)
                {
                    _scope.RecordException(exception);
                }
                else
                {
                    _scope.RecordMetrics(null, null, null, null, null);
                }

                _scope.Dispose();
            }
        }

        public override void OnEvent()
        {
            if ((_completed) || (!_scope._useLatestSemanticConventions))
            {
                return;
            }

            _expectsTerminalResponse = true;
            RecordFirstChunk();
            _eventTime = _elapsedSeconds?.Invoke() ?? _scope._duration.Elapsed.TotalSeconds;

            if (!_receivedEvent)
            {
                _receivedEvent = true;
                _firstChunkTime = _eventTime;
            }
        }

        public override void OnUpdate(StreamingResponseUpdate update)
        {
            if (_completed)
            {
                return;
            }

            var response = update switch
            {
                StreamingResponseCreatedUpdate created => created.Response,
                StreamingResponseInProgressUpdate inProgress => inProgress.Response,
                StreamingResponseQueuedUpdate queued => queued.Response,
                StreamingResponseCompletedUpdate completed => completed.Response,
                StreamingResponseFailedUpdate failed => failed.Response,
                StreamingResponseIncompleteUpdate incomplete => incomplete.Response,
                _ => null,
            };

            if ((_scope._useLatestSemanticConventions) && (response is not null))
            {
                _scope.RecordResponseMetadata(response);
            }

            RecordFirstChunk();

            if ((response is not null) && (update is StreamingResponseCompletedUpdate or StreamingResponseFailedUpdate or StreamingResponseIncompleteUpdate))
            {
                _completed = true;
                _scope.RecordResponseResult(response);
                _scope.Dispose();
            }
            else if (update is StreamingResponseErrorUpdate)
            {
                Finish("stream_error");
            }
            else if ((_scope._useLatestSemanticConventions) && (IsOutputChunk(update)))
            {
                if (_previousOutputTime.HasValue)
                {
                    s_responsesOutputChunk.Record(_eventTime - _previousOutputTime.Value, GetTimingTags());
                }

                _previousOutputTime = _eventTime;
            }
        }

        private TagList GetTimingTags()
        {
            var tags = _scope._commonTags;

            if (_scope._responseMetadata.Model is not null)
            {
                tags.Add(OpenTelemetryConstants.GenAiResponseModelKey, _scope._responseMetadata.Model);
            }

            return tags;
        }

        private void RecordFirstChunk()
        {
            if (_firstChunkTime.HasValue)
            {
                // The first event can supply the response model without buffering subsequent events.
                s_responsesFirstChunk.Record(_firstChunkTime.Value, GetTimingTags());

                if (_scope._activity?.IsAllDataRequested == true)
                {
                    _scope._activity.SetTag(OpenTelemetryConstants.GenAiResponseTimeToFirstChunkKey, _firstChunkTime.Value);
                }

                _firstChunkTime = null;
            }
        }

        private bool IsOutputChunk(StreamingResponseUpdate update)
        {
            var outputIndex = update switch
            {
                StreamingResponseOutputTextDeltaUpdate text => text.OutputIndex,
                StreamingResponseRefusalDeltaUpdate refusal => refusal.OutputIndex,
                StreamingResponseReasoningTextDeltaUpdate reasoning => reasoning.OutputIndex,
                StreamingResponseReasoningSummaryTextDeltaUpdate summary => summary.OutputIndex,
                StreamingResponseFunctionCallArgumentsDeltaUpdate function => function.OutputIndex,
                StreamingResponseCustomToolCallInputDeltaUpdate custom => custom.OutputIndex,
                StreamingResponseMcpCallArgumentsDeltaUpdate mcp => mcp.OutputIndex,
                StreamingResponseCodeInterpreterCallCodeDeltaUpdate code => code.OutputIndex,
                StreamingResponseImageGenerationCallPartialImageUpdate image => image.OutputIndex,
                _ => (int?)null,
            };

            if (outputIndex.HasValue)
            {
                _outputItems.Add(outputIndex.Value);

                return true;
            }

            if (update is StreamingResponseOutputItemDoneUpdate done)
            {
                // A cumulative item is not another chunk when its deltas were already observed.
                return _outputItems.Add(done.OutputIndex);
            }
            // Audio update types are internal, including when these sources are linked into tests.
            // OpenAPI uses dots for transcript events; the generated contract still uses an underscore.
            return (update?.Kind == StreamingResponseUpdateKind.ResponseAudioDelta)
                || (update?.Kind == StreamingResponseUpdateKind.ResponseAudioTranscriptDelta)
                || (update?.Kind.ToString() == "response.audio.transcript.delta");
        }

        public override void OnException(Exception exception)
        {
            if (!_completed)
            {
                RecordFirstChunk();
                _completed = true;
                using var activation = new ActivityActivation(_scope._activity);
                RecordInterruptedFinishReason();
                _scope.RecordException(exception);
                _scope.Dispose();
            }
        }

        public override void Complete(SseCompletionKind completionKind)
        {
            if (!_completed)
            {
                RecordFirstChunk();

                if (completionKind == SseCompletionKind.RawResponse)
                {
                    if (_responseStream is null)
                    {
                        if (_scope._useLatestSemanticConventions)
                        {
                            CompleteRawResponse(null);
                        }
                        else
                        {
                            _completed = true;
                            _scope.Dispose();
                        }
                    }
                }
                else
                {
                    Finish(completionKind == SseCompletionKind.EndOfStream ? "incomplete_stream" : "cancelled");
                }
            }
        }

        private void Finish(string errorType)
        {
            _completed = true;
            using var activation = new ActivityActivation(_scope._activity);
            _scope.RecordMetrics(
                _scope._useLatestSemanticConventions ? _scope._responseMetadata.Model : null,
                _scope._useLatestSemanticConventions ? _scope._responseMetadata.ServiceTier : null,
                errorType, null, null,
                responseSystemFingerprint: _scope._useLatestSemanticConventions ? _scope._responseMetadata.SystemFingerprint : null);

            if (_scope._activity?.IsAllDataRequested == true)
            {
                RecordInterruptedFinishReason();
                _scope.RecordError(errorType, null);
            }

            _scope.Dispose();
        }

        private void RecordInterruptedFinishReason()
        {
            if ((_scope._useLatestSemanticConventions) && (_expectsTerminalResponse) && (_scope._activity?.IsAllDataRequested == true))
            {
                _scope._activity.SetTag(OpenTelemetryConstants.GenAiResponseFinishReasonKey, new[] { "error" });
            }
        }
    }

    private sealed class ActivityActivation : IDisposable
    {
        private readonly Activity _previous = Activity.Current;

        public ActivityActivation(Activity activity)
        {
            if (activity is not null)
            {
                Activity.Current = activity;
            }
        }

        public void Dispose()
        {
            Activity.Current = _previous;
        }
    }
}
