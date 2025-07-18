// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using OpenAI;

namespace OpenAI.Assistants
{
    [Experimental("OPENAI001")]
    public partial class Assistant
    {
        private protected IDictionary<string, BinaryData> _additionalBinaryDataProperties;

        internal Assistant(string id, DateTimeOffset createdAt, string name, string description, string model, string instructions)
        {
            Id = id;
            CreatedAt = createdAt;
            Name = name;
            Description = description;
            Model = model;
            Instructions = instructions;
            Tools = new ChangeTrackingList<ToolDefinition>();
            Metadata = new ChangeTrackingDictionary<string, string>();
        }

        internal Assistant(string id, DateTimeOffset createdAt, string name, string description, string model, string instructions, IReadOnlyList<ToolDefinition> tools, ToolResources toolResources, IReadOnlyDictionary<string, string> metadata, float? temperature, string @object, AssistantResponseFormat responseFormat, float? nucleusSamplingFactor, IDictionary<string, BinaryData> additionalBinaryDataProperties)
        {
            // Plugin customization: ensure initialization of collections
            Id = id;
            CreatedAt = createdAt;
            Name = name;
            Description = description;
            Model = model;
            Instructions = instructions;
            Tools = tools ?? new ChangeTrackingList<ToolDefinition>();
            ToolResources = toolResources;
            Metadata = metadata ?? new ChangeTrackingDictionary<string, string>();
            Temperature = temperature;
            Object = @object;
            ResponseFormat = responseFormat;
            NucleusSamplingFactor = nucleusSamplingFactor;
            _additionalBinaryDataProperties = additionalBinaryDataProperties;
        }

        public string Id { get; }

        public DateTimeOffset CreatedAt { get; }

        public string Name { get; }

        public string Description { get; }

        public string Model { get; }

        public string Instructions { get; }

        public IReadOnlyList<ToolDefinition> Tools { get; }

        public ToolResources ToolResources { get; }

        public IReadOnlyDictionary<string, string> Metadata { get; }

        public float? Temperature { get; }

        internal IDictionary<string, BinaryData> SerializedAdditionalRawData
        {
            get => _additionalBinaryDataProperties;
            set => _additionalBinaryDataProperties = value;
        }
    }
}
