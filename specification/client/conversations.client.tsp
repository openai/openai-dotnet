import "../base/typespec/conversations/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using Azure.ClientGenerator.Core;

namespace OpenAI.Conversations;

@@clientName(OpenAI.Conversations, "ConversationsClient", "csharp");
@@clientNamespace(OpenAI.Conversations, "OpenAI.Conversations", "csharp"); 
@@dynamicModel(OpenAI.Conversations, "csharp");

@@alternateType(ConversationItemCollectionOptions.limit, int32);

// Move operations into their corresponding interface
@@clientLocation(createConversation, "Conversations");
@@clientLocation(getConversation, "Conversations");
@@clientLocation(updateConversation, "Conversations");
@@clientLocation(deleteConversation, "Conversations");
@@clientLocation(listConversationItems, "Conversations");
@@clientLocation(createConversationItems, "Conversations");
@@clientLocation(getConversationItem, "Conversations");
@@clientLocation(deleteConversationItem, "Conversations");

@@clientName(CreateConversationBody, "ConversationCreationOptions");
@@clientName(UpdateConversationBody, "ConversationUpdateOptions");
@@clientName(DeletedConversationResource, "ConversationDeletionResult");
@@clientName(DeletedConversationResource.id, "ConversationId");
@@clientName(ConversationItemCollectionOrder.asc, "Ascending");
@@clientName(ConversationItemCollectionOrder.desc, "Descending");

// Not generate convenience methods for these operations
@@convenientAPI(listConversationItems, false);
@@convenientAPI(createConversationItems, false);
@@convenientAPI(getConversationItem, false);
@@convenientAPI(deleteConversationItem, false);

// Rename method
@@clientName(listConversationItems, "GetConversationItems");

@@clientName(IncludeEnum.`computer_call_output.output.image_url`, "ComputerCallOutputImageUri");
@@clientName(IncludeEnum.`message.input_image.image_url`, "MessageInputImageUri");
@@clientName(IncludeEnum.`message.output_text.logprobs`, "MessageOutputTextLogProbabilities");
