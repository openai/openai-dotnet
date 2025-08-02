import "../common";
import "./models.tsp";

using TypeSpec.Http;
using TypeSpec.OpenAPI;

namespace OpenAI;

@route("realtime")
@tag("Realtime")
interface Realtime {
  @summary("Starts a real-time session for conversation or transcription.")
  startRealtimeSession(
    ...RealtimeBetaHeader,
    @body requestMessages: RealtimeClientEvent[],
  ): RealtimeServerEvent[];

  @post
  @route("sessions")
  @operationId("create-realtime-session")
  @summary("""
    Create an ephemeral API token for use in client-side applications with the Realtime API. Can be configured with the same session parameters as the session.update client event.
    
    It responds with a session object, plus a client_secret key which contains a usable ephemeral API token that can be used to authenticate browser clients for the Realtime API.
    """)
  createEphemeralToken(
    @body request: RealtimeSessionCreateRequest,
  ): RealtimeSessionCreateResponse | ErrorResponse;

  @post
  @route("transcription_sessions")
  @operationId("create-realtime-transcription-session")
  @summary("""
    Create an ephemeral API token for use in client-side applications with the Realtime API specifically for realtime transcriptions. Can be configured with the same session parameters as the transcription_session.update client event.
    
    It responds with a session object, plus a client_secret key which contains a usable ephemeral API token that can be used to authenticate browser clients for the Realtime API.
    """)
  createEphemeralTranscriptionToken(
    @body request: RealtimeTranscriptionSessionCreateRequest,
  ): RealtimeTranscriptionSessionCreateResponse | ErrorResponse;
}

alias RealtimeBetaHeader = {
  @header("OpenAI-Beta") openAIBeta: "realtime=v1";
};
