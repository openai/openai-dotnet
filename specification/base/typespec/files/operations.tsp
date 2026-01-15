import "@typespec/http";
import "@typespec/openapi";
import "./models.tsp";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
namespace OpenAI;
/** Returns a list of files. */
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List files")
@get
@route("/files")
@extension(
  "x-oaiMeta",
  #{
    name: "List files",
    group: "files",
    returns: "A list of [File](https://platform.openai.com/docs/api-reference/files/object) objects.",
    examples: #{
      response: """
        {
          "object": "list",
          "data": [
            {
              "id": "file-abc123",
              "object": "file",
              "bytes": 175,
              "created_at": 1613677385,
              "expires_at": 1677614202,
              "filename": "salesOverview.pdf",
              "purpose": "assistants",
            },
            {
              "id": "file-abc456",
              "object": "file",
              "bytes": 140,
              "created_at": 1613779121,
              "expires_at": 1677614202,
              "filename": "puppy.jsonl",
              "purpose": "fine-tune",
            }
          ],
          "first_id": "file-abc123",
          "last_id": "file-abc456",
          "has_more": false
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/files \\
            -H "Authorization: Bearer $OPENAI_API_KEY"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.files.list()
          page = page.data[0]
          print(page)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const fileObject of client.files.list()) {
            console.log(fileObject);
          }
          """,
        go: """
          package main
          
          import (
            "context"
            "fmt"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            page, err := client.Files.List(context.TODO(), openai.FileListParams{
          
            })
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", page)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.files.FileListPage;
          import com.openai.models.files.FileListParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileListPage page = client.files().list();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.files.list
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Files")
op listFiles(
  /** Only return files with the given purpose. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  purpose?: string,

  /** A limit on the number of objects to be returned. Limit can range between 1 and 10,000, and the default is 10,000. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  limit?: integer = 10000,

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-closed-literal-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: "asc" | "desc" = "desc",

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  after?: string,
): ListFilesResponse;

/**Upload a file that can be used across various endpoints. Individual files
can be up to 512 MB, and the size of all files uploaded by one organization
can be up to 1 TB.
- The Assistants API supports files up to 2 million tokens and of specific
file types. See the [Assistants Tools guide](https://platform.openai.com/docs/assistants/tools) for
details.
- The Fine-tuning API only supports `.jsonl` files. The input also has
certain required formats for fine-tuning
[chat](https://platform.openai.com/docs/api-reference/fine-tuning/chat-input) or
[completions](https://platform.openai.com/docs/api-reference/fine-tuning/completions-input) models.
- The Batch API only supports `.jsonl` files up to 200 MB in size. The input
also has a specific required
[format](https://platform.openai.com/docs/api-reference/batch/request-input).
Please [contact us](https://help.openai.com/) if you need to increase these
storage limits.*/
#suppress "@azure-tools/typespec-azure-core/byos" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Upload file")
@post
@route("/files")
@extension(
  "x-oaiMeta",
  #{
    name: "Upload file",
    group: "files",
    returns: "The uploaded [File](https://platform.openai.com/docs/api-reference/files/object) object.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "file",
          "bytes": 120000,
          "created_at": 1677610602,
          "expires_at": 1677614202,
          "filename": "mydata.jsonl",
          "purpose": "fine-tune",
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/files \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -F purpose="fine-tune" \\
            -F file="@mydata.jsonl"
            -F expires_after[anchor]="created_at"
            -F expires_after[seconds]=2592000
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          file_object = client.files.create(
              file=b"raw file contents",
              purpose="assistants",
          )
          print(file_object.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const fileObject = await client.files.create({
            file: fs.createReadStream('fine-tune.jsonl'),
            purpose: 'assistants',
          });
          
          console.log(fileObject.id);
          """,
        go: """
          package main
          
          import (
            "bytes"
            "context"
            "fmt"
            "io"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            fileObject, err := client.Files.New(context.TODO(), openai.FileNewParams{
              File: io.Reader(bytes.NewBuffer([]byte("some file contents"))),
              Purpose: openai.FilePurposeAssistants,
            })
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", fileObject.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.files.FileCreateParams;
          import com.openai.models.files.FileObject;
          import com.openai.models.files.FilePurpose;
          import java.io.ByteArrayInputStream;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileCreateParams params = FileCreateParams.builder()
                      .file(ByteArrayInputStream("some content".getBytes()))
                      .purpose(FilePurpose.ASSISTANTS)
                      .build();
                  FileObject fileObject = client.files().create(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          file_object = openai.files.create(file: Pathname(__FILE__), purpose: :assistants)
          
          puts(file_object)
          """,
      },
    },
  }
)
@tag("Files")
op createFile(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @header
  contentType: "multipart/form-data",

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @multipartBody
  body: CreateFileRequest,
): OpenAIFile;

/** Delete a file and remove it from all vector stores. */
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete file")
@delete
@route("/files/{file_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Delete file",
    group: "files",
    returns: "Deletion status.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "file",
          "deleted": true
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/files/file-abc123 \\
            -X DELETE \\
            -H "Authorization: Bearer $OPENAI_API_KEY"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          file_deleted = client.files.delete(
              "file_id",
          )
          print(file_deleted.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const fileDeleted = await client.files.delete('file_id');
          
          console.log(fileDeleted.id);
          """,
        go: """
          package main
          
          import (
            "context"
            "fmt"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            fileDeleted, err := client.Files.Delete(context.TODO(), "file_id")
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", fileDeleted.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.files.FileDeleteParams;
          import com.openai.models.files.FileDeleted;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileDeleted fileDeleted = client.files().delete("file_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          file_deleted = openai.files.delete("file_id")
          
          puts(file_deleted)
          """,
      },
    },
  }
)
@tag("Files")
op deleteFile(
  /** The ID of the file to use for this request. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): DeleteFileResponse;

/** Returns information about a specific file. */
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve file")
@get
@route("/files/{file_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve file",
    group: "files",
    returns: "The [File](https://platform.openai.com/docs/api-reference/files/object) object matching the specified ID.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "file",
          "bytes": 120000,
          "created_at": 1677610602,
          "expires_at": 1677614202,
          "filename": "mydata.jsonl",
          "purpose": "fine-tune",
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/files/file-abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          file_object = client.files.retrieve(
              "file_id",
          )
          print(file_object.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const fileObject = await client.files.retrieve('file_id');
          
          console.log(fileObject.id);
          """,
        go: """
          package main
          
          import (
            "context"
            "fmt"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            fileObject, err := client.Files.Get(context.TODO(), "file_id")
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", fileObject.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.files.FileObject;
          import com.openai.models.files.FileRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileObject fileObject = client.files().retrieve("file_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          file_object = openai.files.retrieve("file_id")
          
          puts(file_object)
          """,
      },
    },
  }
)
@tag("Files")
op retrieveFile(
  /** The ID of the file to use for this request. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): OpenAIFile;

/** Returns the contents of the specified file. */
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve file content")
@get
@route("/files/{file_id}/content")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve file content",
    group: "files",
    returns: "The file content.",
    examples: #{
      response: "",
      request: #{
        curl: """
          curl https://api.openai.com/v1/files/file-abc123/content \\
            -H "Authorization: Bearer $OPENAI_API_KEY" > file.jsonl
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          response = client.files.content(
              "file_id",
          )
          print(response)
          content = response.read()
          print(content)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const response = await client.files.content('file_id');
          
          console.log(response);
          
          const content = await response.blob();
          console.log(content);
          """,
        go: """
          package main
          
          import (
            "context"
            "fmt"
          
            "github.com/openai/openai-go"
            "github.com/openai/openai-go/option"
          )
          
          func main() {
            client := openai.NewClient(
              option.WithAPIKey("My API Key"),
            )
            response, err := client.Files.Content(context.TODO(), "file_id")
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", response)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.core.http.HttpResponse;
          import com.openai.models.files.FileContentParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  HttpResponse response = client.files().content("file_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          response = openai.files.content("file_id")
          
          puts(response)
          """,
      },
    },
  }
)
@tag("Files")
op downloadFile(
  /** The ID of the file to use for this request. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): Body<string>;
