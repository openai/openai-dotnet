// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace OpenAI.Chat;

public class GetChatCompletionsOptions
{
    public GetChatCompletionsOptions(string model)
    {
        Model = model;
    }

    public string After { get; set; }

    public int? Limit { get; set; }

    public string Order { get; set; }

    public IDictionary<string, string> Metadata { get; set; }

    public string Model { get; set; }

    public static GetChatCompletionsOptions Create(ChatClient client)
    {
        return new GetChatCompletionsOptions(client.Model);
    }
}