import "../base/typespec/files/main.tsp";
import "@azure-tools/typespec-client-generator-core";

using OpenAI;
using Azure.ClientGenerator.Core;

@@alternateType(FileExpirationAfter.seconds, int32);
@@alternateType(listFiles::parameters.limit, int32);

@@clientName(createFile, "UploadFile");
@@clientName(listFiles, "GetFiles");
@@clientName(retrieveFile, "GetFile");

@@clientLocation(createFile, "Files");
@@clientLocation(listFiles, "Files");
@@clientLocation(retrieveFile, "Files");
@@clientLocation(downloadFile, "Files");
@@clientLocation(deleteFile, "Files");

@@alternateType(OpenAIFile.bytes, int32 | null);
