// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;

namespace OpenAI.Chat
{
    public partial class ChatClient
    {
        private readonly Uri _endpoint;
        private const string AuthorizationHeader = "Authorization";
        private readonly ApiKeyCredential _keyCredential;
        private const string AuthorizationApiKeyPrefix = "Bearer";

        protected ChatClient()
        {
        }

        public ClientPipeline Pipeline { get; }
    }
}
