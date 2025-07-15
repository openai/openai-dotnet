import "../base/typespec/batch/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@access(BatchRequestInput, Access.public);
@@usage(BatchRequestInput, Usage.input);

@@access(BatchRequestOutput, Access.public);
@@usage(BatchRequestOutput, Usage.output);

@@clientName(Batches.listBatches, "GetBatches");
@@clientName(Batches.retrieveBatch, "GetBatch");
