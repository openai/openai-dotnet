import "../base/typespec/files/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@clientName(Files.createFile, "UploadFile");
@@clientName(Files.listFiles, "GetFiles");
@@clientName(Files.retrieveFile, "GetFile");
