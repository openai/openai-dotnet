import "../base/typespec/messages/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

//
@@access(MessageContent, Access.public);
@@usage(MessageContent, Usage.input | Usage.output);

@@access(MessageContentImageFileObject, Access.public);
@@usage(MessageContentImageFileObject, Usage.input | Usage.output);

@@access(MessageContentTextObject, Access.public);
@@usage(MessageContentTextObject, Usage.input | Usage.output);

@@access(MessageContentImageUrlObject, Access.public);
@@usage(MessageContentImageUrlObject, Usage.input | Usage.output);

@@access(MessageContentRefusalObject, Access.public);
@@usage(MessageContentRefusalObject, Usage.input | Usage.output);

@@access(MessageRequestContentTextObject, Access.public);
@@usage(MessageRequestContentTextObject, Usage.input | Usage.output);

@@access(MessageContentTextObjectAnnotation, Access.public);
@@usage(MessageContentTextObjectAnnotation, Usage.input | Usage.output);

@@access(MessageContentTextAnnotationsFileCitationObject, Access.public);
@@usage(MessageContentTextAnnotationsFileCitationObject, Usage.input | Usage.output);

@@access(MessageContentTextAnnotationsFilePathObject, Access.public);
@@usage(MessageContentTextAnnotationsFilePathObject, Usage.input | Usage.output);

//
@@access(MessageDeltaContent, Access.public);
@@usage(MessageDeltaContent, Usage.output);

@@access(MessageDeltaContentImageFileObject, Access.public);
@@usage(MessageDeltaContentImageFileObject, Usage.output);

@@access(MessageDeltaContentImageUrlObject, Access.public);
@@usage(MessageDeltaContentImageUrlObject, Usage.output);

@@access(MessageDeltaContentTextObject, Access.public);
@@usage(MessageDeltaContentTextObject, Usage.output);

//
@@access(MessageDeltaObject, Access.public);
@@usage(MessageDeltaObject, Usage.output);

//
@@access(MessageDeltaTextContentAnnotation, Access.public);
@@usage(MessageDeltaTextContentAnnotation, Usage.output);

@@access(MessageDeltaContentTextAnnotationsFileCitationObject, Access.public);
@@usage(MessageDeltaContentTextAnnotationsFileCitationObject, Usage.output);

@@access(MessageDeltaContentTextAnnotationsFilePathObject, Access.public);
@@usage(MessageDeltaContentTextAnnotationsFilePathObject, Usage.output);

@@visibility(ListMessagesResponse.data, Lifecycle.Read);

@@visibility(MessageObject.content, Lifecycle.Read);
@@visibility(MessageObject.metadata, Lifecycle.Read);
