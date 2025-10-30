import "../base/typespec/conversations/main.tsp";
import "@azure-tools/typespec-client-generator-core";
import "@typespec/http-client-csharp";

using OpenAI;
using Azure.ClientGenerator.Core;
using TypeSpec.HttpClient.CSharp;

// Move operations into their corresponding interface
@@clientLocation(createConversation, "Conversations");
@@clientLocation(getConversation, "Conversations");
@@clientLocation(updateConversation, "Conversations");
@@clientLocation(deleteConversation, "Conversations");
@@clientLocation(listConversationItems, "Conversations");
@@clientLocation(createConversationItems, "Conversations");
@@clientLocation(getConversationItem, "Conversations");
@@clientLocation(deleteConversationItem, "Conversations");

// Not generate convenience methods for these operations
@@convenientAPI(createConversation, false);
@@convenientAPI(getConversation, false);
@@convenientAPI(updateConversation, false);
@@convenientAPI(deleteConversation, false);
@@convenientAPI(listConversationItems, false);
@@convenientAPI(createConversationItems, false);
@@convenientAPI(getConversationItem, false);
@@convenientAPI(deleteConversationItem, false);

// Rename method
@@clientName(listConversationItems, "GetConversationItems");