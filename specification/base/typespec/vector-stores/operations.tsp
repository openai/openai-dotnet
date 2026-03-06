import "@typespec/http";
import "@typespec/openapi";
import "../common/models.tsp";
import "./models.tsp";
using TypeSpec.Http;
using TypeSpec.OpenAPI;
namespace OpenAI;
/** Returns a list of vector stores. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List vector stores")
@get
@route("/vector_stores")
@extension("x-ms-list", true)
@list
@extension(
  "x-oaiMeta",
  #{
    name: "List vector stores",
    group: "vector_stores",
    returns: "A list of [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) objects.",
    examples: #{
      response: """
        {
          "object": "list",
          "data": [
            {
              "id": "vs_abc123",
              "object": "vector_store",
              "created_at": 1699061776,
              "name": "Support FAQ",
              "description": "Contains commonly asked questions and answers, organized by topic.",
              "bytes": 139920,
              "file_counts": {
                "in_progress": 0,
                "completed": 3,
                "failed": 0,
                "cancelled": 0,
                "total": 3
              }
            },
            {
              "id": "vs_abc456",
              "object": "vector_store",
              "created_at": 1699061776,
              "name": "Support FAQ v2",
              "description": null,
              "bytes": 139920,
              "file_counts": {
                "in_progress": 0,
                "completed": 3,
                "failed": 0,
                "cancelled": 0,
                "total": 3
              }
            }
          ],
          "first_id": "vs_abc123",
          "last_id": "vs_abc456",
          "has_more": false
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.vector_stores.list()
          page = page.data[0]
          print(page.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const vectorStore of client.vectorStores.list()) {
            console.log(vectorStore.id);
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
            page, err := client.VectorStores.List(context.TODO(), openai.VectorStoreListParams{
          
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
          import com.openai.models.vectorstores.VectorStoreListPage;
          import com.openai.models.vectorstores.VectorStoreListParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStoreListPage page = client.vectorStores().list();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.vector_stores.list
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Vector stores")
op listVectorStores(
  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  limit?: integer = 20,

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: OrderEnum = OrderEnum.desc,

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @continuationToken
  @extension("x-ms-list-continuation-token", true)
  after?: string,

  /** A cursor for use in pagination. `before` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, starting with obj_foo, your subsequent call can include before=obj_foo in order to fetch the previous page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  before?: string,
): ListVectorStoresResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Create a vector store. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create vector store")
@post
@route("/vector_stores")
@extension(
  "x-oaiMeta",
  #{
    name: "Create vector store",
    group: "vector_stores",
    returns: "A [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) object.",
    examples: #{
      response: """
        {
          "id": "vs_abc123",
          "object": "vector_store",
          "created_at": 1699061776,
          "name": "Support FAQ",
          "description": "Contains commonly asked questions and answers, organized by topic.",
          "bytes": 139920,
          "file_counts": {
            "in_progress": 0,
            "completed": 3,
            "failed": 0,
            "cancelled": 0,
            "total": 3
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2" \\
            -d '{
              "name": "Support FAQ"
            }'
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store = client.vector_stores.create()
          print(vector_store.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStore = await client.vectorStores.create();
          
          console.log(vectorStore.id);
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
            vectorStore, err := client.VectorStores.New(context.TODO(), openai.VectorStoreNewParams{
          
            })
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStore.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.VectorStore;
          import com.openai.models.vectorstores.VectorStoreCreateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStore vectorStore = client.vectorStores().create();
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store = openai.vector_stores.create
          
          puts(vector_store)
          """,
      },
    },
  }
)
@tag("Vector stores")
op createVectorStore(
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateVectorStoreRequest,
): VectorStoreObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Delete a vector store. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete vector store")
@delete
@route("/vector_stores/{vector_store_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Delete vector store",
    group: "vector_stores",
    returns: "Deletion status",
    examples: #{
      response: """
        {
          id: "vs_abc123",
          object: "vector_store.deleted",
          deleted: true
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2" \\
            -X DELETE
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_deleted = client.vector_stores.delete(
              "vector_store_id",
          )
          print(vector_store_deleted.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreDeleted = await client.vectorStores.delete('vector_store_id');
          
          console.log(vectorStoreDeleted.id);
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
            vectorStoreDeleted, err := client.VectorStores.Delete(context.TODO(), "vector_store_id")
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreDeleted.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.VectorStoreDeleteParams;
          import com.openai.models.vectorstores.VectorStoreDeleted;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStoreDeleted vectorStoreDeleted = client.vectorStores().delete("vector_store_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_deleted = openai.vector_stores.delete("vector_store_id")
          
          puts(vector_store_deleted)
          """,
      },
    },
  }
)
@tag("Vector stores")
op deleteVectorStore(
  /** The ID of the vector store to delete. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,
): DeleteVectorStoreResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Retrieves a vector store. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve vector store")
@get
@route("/vector_stores/{vector_store_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve vector store",
    group: "vector_stores",
    returns: "The [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) object matching the specified ID.",
    examples: #{
      response: """
        {
          "id": "vs_abc123",
          "object": "vector_store",
          "created_at": 1699061776
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store = client.vector_stores.retrieve(
              "vector_store_id",
          )
          print(vector_store.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStore = await client.vectorStores.retrieve('vector_store_id');
          
          console.log(vectorStore.id);
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
            vectorStore, err := client.VectorStores.Get(context.TODO(), "vector_store_id")
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStore.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.VectorStore;
          import com.openai.models.vectorstores.VectorStoreRetrieveParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStore vectorStore = client.vectorStores().retrieve("vector_store_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store = openai.vector_stores.retrieve("vector_store_id")
          
          puts(vector_store)
          """,
      },
    },
  }
)
@tag("Vector stores")
op getVectorStore(
  /** The ID of the vector store to retrieve. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,
): VectorStoreObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Modifies a vector store. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Modify vector store")
@post
@route("/vector_stores/{vector_store_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Modify vector store",
    group: "vector_stores",
    returns: "The modified [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object) object.",
    examples: #{
      response: """
        {
          "id": "vs_abc123",
          "object": "vector_store",
          "created_at": 1699061776,
          "name": "Support FAQ",
          "description": "Contains commonly asked questions and answers, organized by topic.",
          "bytes": 139920,
          "file_counts": {
            "in_progress": 0,
            "completed": 3,
            "failed": 0,
            "cancelled": 0,
            "total": 3
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
            -d '{
              "name": "Support FAQ"
            }'
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store = client.vector_stores.update(
              vector_store_id="vector_store_id",
          )
          print(vector_store.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStore = await client.vectorStores.update('vector_store_id');
          
          console.log(vectorStore.id);
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
            vectorStore, err := client.VectorStores.Update(
              context.TODO(),
              "vector_store_id",
              openai.VectorStoreUpdateParams{
          
              },
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStore.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.VectorStore;
          import com.openai.models.vectorstores.VectorStoreUpdateParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStore vectorStore = client.vectorStores().update("vector_store_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store = openai.vector_stores.update("vector_store_id")
          
          puts(vector_store)
          """,
      },
    },
  }
)
@tag("Vector stores")
op modifyVectorStore(
  /** The ID of the vector store to modify. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: UpdateVectorStoreRequest,
): VectorStoreObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Create a vector store file batch. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create vector store file batch")
@post
@route("/vector_stores/{vector_store_id}/file_batches")
@extension(
  "x-oaiMeta",
  #{
    name: "Create vector store file batch",
    group: "vector_stores",
    returns: "A [vector store file batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/batch-object) object.",
    examples: #{
      response: """
        {
          "id": "vsfb_abc123",
          "object": "vector_store.file_batch",
          "created_at": 1699061776,
          "vector_store_id": "vs_abc123",
          "status": "in_progress",
          "file_counts": {
            "in_progress": 1,
            "completed": 1,
            "failed": 0,
            "cancelled": 0,
            "total": 0,
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/file_batches \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -H "Content-Type: application/json \\
              -H "OpenAI-Beta: assistants=v2" \\
              -d '{
                "files": [
                  {
                    "file_id": "file-abc123",
                    "attributes": {"category": "finance"}
                  },
                  {
                    "file_id": "file-abc456",
                    "chunking_strategy": {
                      "type": "static",
                      "max_chunk_size_tokens": 1200,
                      "chunk_overlap_tokens": 200
                    }
                  }
                ]
              }'
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file_batch = client.vector_stores.file_batches.create(
              vector_store_id="vs_abc123",
          )
          print(vector_store_file_batch.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFileBatch = await client.vectorStores.fileBatches.create('vs_abc123');
          
          console.log(vectorStoreFileBatch.id);
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
            vectorStoreFileBatch, err := client.VectorStores.FileBatches.New(
              context.TODO(),
              "vs_abc123",
              openai.VectorStoreFileBatchNewParams{
          
              },
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFileBatch.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.filebatches.FileBatchCreateParams;
          import com.openai.models.vectorstores.filebatches.VectorStoreFileBatch;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStoreFileBatch vectorStoreFileBatch = client.vectorStores().fileBatches().create("vs_abc123");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file_batch = openai.vector_stores.file_batches.create("vs_abc123")
          
          puts(vector_store_file_batch)
          """,
      },
    },
  }
)
@tag("Vector stores")
op createVectorStoreFileBatch(
  /** The ID of the vector store for which to create a File Batch. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateVectorStoreFileBatchRequest,
): VectorStoreFileBatchObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Retrieves a vector store file batch. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve vector store file batch")
@get
@route("/vector_stores/{vector_store_id}/file_batches/{batch_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve vector store file batch",
    group: "vector_stores",
    returns: "The [vector store file batch](https://platform.openai.com/docs/api-reference/vector-stores-file-batches/batch-object) object.",
    examples: #{
      response: """
        {
          "id": "vsfb_abc123",
          "object": "vector_store.file_batch",
          "created_at": 1699061776,
          "vector_store_id": "vs_abc123",
          "status": "in_progress",
          "file_counts": {
            "in_progress": 1,
            "completed": 1,
            "failed": 0,
            "cancelled": 0,
            "total": 0,
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files_batches/vsfb_abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file_batch = client.vector_stores.file_batches.retrieve(
              batch_id="vsfb_abc123",
              vector_store_id="vs_abc123",
          )
          print(vector_store_file_batch.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFileBatch = await client.vectorStores.fileBatches.retrieve('vsfb_abc123', {
            vector_store_id: 'vs_abc123',
          });
          
          console.log(vectorStoreFileBatch.id);
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
            vectorStoreFileBatch, err := client.VectorStores.FileBatches.Get(
              context.TODO(),
              "vs_abc123",
              "vsfb_abc123",
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFileBatch.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.filebatches.FileBatchRetrieveParams;
          import com.openai.models.vectorstores.filebatches.VectorStoreFileBatch;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileBatchRetrieveParams params = FileBatchRetrieveParams.builder()
                      .vectorStoreId("vs_abc123")
                      .batchId("vsfb_abc123")
                      .build();
                  VectorStoreFileBatch vectorStoreFileBatch = client.vectorStores().fileBatches().retrieve(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file_batch = openai.vector_stores.file_batches.retrieve("vsfb_abc123", vector_store_id: "vs_abc123")
          
          puts(vector_store_file_batch)
          """,
      },
    },
  }
)
@tag("Vector stores")
op getVectorStoreFileBatch(
  /** The ID of the vector store that the file batch belongs to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file batch being retrieved. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  batch_id: string,
): VectorStoreFileBatchObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Cancel a vector store file batch. This attempts to cancel the processing of files in this batch as soon as possible. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Cancel vector store file batch")
@post
@route("/vector_stores/{vector_store_id}/file_batches/{batch_id}/cancel")
@extension(
  "x-oaiMeta",
  #{
    name: "Cancel vector store file batch",
    group: "vector_stores",
    returns: "The modified vector store file batch object.",
    examples: #{
      response: """
        {
          "id": "vsfb_abc123",
          "object": "vector_store.file_batch",
          "created_at": 1699061776,
          "vector_store_id": "vs_abc123",
          "status": "in_progress",
          "file_counts": {
            "in_progress": 12,
            "completed": 3,
            "failed": 0,
            "cancelled": 0,
            "total": 15,
          }
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files_batches/vsfb_abc123/cancel \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2" \\
            -X POST
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file_batch = client.vector_stores.file_batches.cancel(
              batch_id="batch_id",
              vector_store_id="vector_store_id",
          )
          print(vector_store_file_batch.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFileBatch = await client.vectorStores.fileBatches.cancel('batch_id', {
            vector_store_id: 'vector_store_id',
          });
          
          console.log(vectorStoreFileBatch.id);
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
            vectorStoreFileBatch, err := client.VectorStores.FileBatches.Cancel(
              context.TODO(),
              "vector_store_id",
              "batch_id",
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFileBatch.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.filebatches.FileBatchCancelParams;
          import com.openai.models.vectorstores.filebatches.VectorStoreFileBatch;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileBatchCancelParams params = FileBatchCancelParams.builder()
                      .vectorStoreId("vector_store_id")
                      .batchId("batch_id")
                      .build();
                  VectorStoreFileBatch vectorStoreFileBatch = client.vectorStores().fileBatches().cancel(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file_batch = openai.vector_stores.file_batches.cancel("batch_id", vector_store_id: "vector_store_id")
          
          puts(vector_store_file_batch)
          """,
      },
    },
  }
)
@tag("Vector stores")
op cancelVectorStoreFileBatch(
  /** The ID of the vector store that the file batch belongs to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file batch to cancel. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  batch_id: string,
): VectorStoreFileBatchObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Returns a list of vector store files in a batch. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List vector store files in a batch")
@get
@route("/vector_stores/{vector_store_id}/file_batches/{batch_id}/files")
@extension("x-ms-list", true)
@list
@extension(
  "x-oaiMeta",
  #{
    name: "List vector store files in a batch",
    group: "vector_stores",
    returns: "A list of [vector store file](https://platform.openai.com/docs/api-reference/vector-stores-files/file-object) objects.",
    examples: #{
      response: """
        {
          "object": "list",
          "data": [
            {
              "id": "file-abc123",
              "object": "vector_store.file",
              "created_at": 1699061776,
              "vector_store_id": "vs_abc123"
            },
            {
              "id": "file-abc456",
              "object": "vector_store.file",
              "created_at": 1699061776,
              "vector_store_id": "vs_abc123"
            }
          ],
          "first_id": "file-abc123",
          "last_id": "file-abc456",
          "has_more": false
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files_batches/vsfb_abc123/files \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.vector_stores.file_batches.list_files(
              batch_id="batch_id",
              vector_store_id="vector_store_id",
          )
          page = page.data[0]
          print(page.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const vectorStoreFile of client.vectorStores.fileBatches.listFiles('batch_id', {
            vector_store_id: 'vector_store_id',
          })) {
            console.log(vectorStoreFile.id);
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
            page, err := client.VectorStores.FileBatches.ListFiles(
              context.TODO(),
              "vector_store_id",
              "batch_id",
              openai.VectorStoreFileBatchListFilesParams{
          
              },
            )
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
          import com.openai.models.vectorstores.filebatches.FileBatchListFilesPage;
          import com.openai.models.vectorstores.filebatches.FileBatchListFilesParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileBatchListFilesParams params = FileBatchListFilesParams.builder()
                      .vectorStoreId("vector_store_id")
                      .batchId("batch_id")
                      .build();
                  FileBatchListFilesPage page = client.vectorStores().fileBatches().listFiles(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.vector_stores.file_batches.list_files("batch_id", vector_store_id: "vector_store_id")
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Vector stores")
op listFilesInVectorStoreBatch(
  /** The ID of the vector store that the files belong to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file batch that the files belong to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  batch_id: string,

  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  limit?: integer = 20,

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: OrderEnum = OrderEnum.desc,

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @continuationToken
  @extension("x-ms-list-continuation-token", true)
  after?: string,

  /** A cursor for use in pagination. `before` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, starting with obj_foo, your subsequent call can include before=obj_foo in order to fetch the previous page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  before?: string,

  /** Filter by file status. One of `in_progress`, `completed`, `failed`, `cancelled`. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  filter?: ListVectorStoreFilesFilter,
): ListVectorStoreFilesResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Returns a list of vector store files. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("List vector store files")
@get
@route("/vector_stores/{vector_store_id}/files")
@extension("x-ms-list", true)
@list
@extension(
  "x-oaiMeta",
  #{
    name: "List vector store files",
    group: "vector_stores",
    returns: "A list of [vector store file](https://platform.openai.com/docs/api-reference/vector-stores-files/file-object) objects.",
    examples: #{
      response: """
        {
          "object": "list",
          "data": [
            {
              "id": "file-abc123",
              "object": "vector_store.file",
              "created_at": 1699061776,
              "vector_store_id": "vs_abc123"
            },
            {
              "id": "file-abc456",
              "object": "vector_store.file",
              "created_at": 1699061776,
              "vector_store_id": "vs_abc123"
            }
          ],
          "first_id": "file-abc123",
          "last_id": "file-abc456",
          "has_more": false
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.vector_stores.files.list(
              vector_store_id="vector_store_id",
          )
          page = page.data[0]
          print(page.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const vectorStoreFile of client.vectorStores.files.list('vector_store_id')) {
            console.log(vectorStoreFile.id);
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
            page, err := client.VectorStores.Files.List(
              context.TODO(),
              "vector_store_id",
              openai.VectorStoreFileListParams{
          
              },
            )
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
          import com.openai.models.vectorstores.files.FileListPage;
          import com.openai.models.vectorstores.files.FileListParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileListPage page = client.vectorStores().files().list("vector_store_id");
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.vector_stores.files.list("vector_store_id")
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Vector stores")
op listVectorStoreFiles(
  /** The ID of the vector store that the files belong to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** A limit on the number of objects to be returned. Limit can range between 1 and 100, and the default is 20. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  limit?: integer = 20,

  /** Sort order by the `created_at` timestamp of the objects. `asc` for ascending order and `desc` for descending order. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  order?: OrderEnum = OrderEnum.desc,

  /** A cursor for use in pagination. `after` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, ending with obj_foo, your subsequent call can include after=obj_foo in order to fetch the next page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  @continuationToken
  @extension("x-ms-list-continuation-token", true)
  after?: string,

  /** A cursor for use in pagination. `before` is an object ID that defines your place in the list. For instance, if you make a list request and receive 100 objects, starting with obj_foo, your subsequent call can include before=obj_foo in order to fetch the previous page of the list. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  before?: string,

  /** Filter by file status. One of `in_progress`, `completed`, `failed`, `cancelled`. */
  #suppress "@azure-tools/typespec-azure-core/no-query-explode" "Auto-suppressed warnings non-applicable rules during import."
  @query(#{ explode: true })
  filter?: ListVectorStoreFilesFilter,
): ListVectorStoreFilesResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Create a vector store file by attaching a [File](https://platform.openai.com/docs/api-reference/files) to a [vector store](https://platform.openai.com/docs/api-reference/vector-stores/object). */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Create vector store file")
@post
@route("/vector_stores/{vector_store_id}/files")
@extension(
  "x-oaiMeta",
  #{
    name: "Create vector store file",
    group: "vector_stores",
    returns: "A [vector store file](https://platform.openai.com/docs/api-reference/vector-stores-files/file-object) object.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "vector_store.file",
          "created_at": 1699061776,
          "usage_bytes": 1234,
          "vector_store_id": "vs_abcd",
          "status": "completed",
          "last_error": null
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files \\
              -H "Authorization: Bearer $OPENAI_API_KEY" \\
              -H "Content-Type: application/json" \\
              -H "OpenAI-Beta: assistants=v2" \\
              -d '{
                "file_id": "file-abc123"
              }'
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file = client.vector_stores.files.create(
              vector_store_id="vs_abc123",
              file_id="file_id",
          )
          print(vector_store_file.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFile = await client.vectorStores.files.create('vs_abc123', { file_id: 'file_id' });
          
          console.log(vectorStoreFile.id);
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
            vectorStoreFile, err := client.VectorStores.Files.New(
              context.TODO(),
              "vs_abc123",
              openai.VectorStoreFileNewParams{
                FileID: "file_id",
              },
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFile.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.files.FileCreateParams;
          import com.openai.models.vectorstores.files.VectorStoreFile;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileCreateParams params = FileCreateParams.builder()
                      .vectorStoreId("vs_abc123")
                      .fileId("file_id")
                      .build();
                  VectorStoreFile vectorStoreFile = client.vectorStores().files().create(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file = openai.vector_stores.files.create("vs_abc123", file_id: "file_id")
          
          puts(vector_store_file)
          """,
      },
    },
  }
)
@tag("Vector stores")
op createVectorStoreFile(
  /** The ID of the vector store for which to create a File. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: CreateVectorStoreFileRequest,
): VectorStoreFileObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Delete a vector store file. This will remove the file from the vector store but the file itself will not be deleted. To delete the file, use the [delete file](https://platform.openai.com/docs/api-reference/files/delete) endpoint. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Delete vector store file")
@delete
@route("/vector_stores/{vector_store_id}/files/{file_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Delete vector store file",
    group: "vector_stores",
    returns: "Deletion status",
    examples: #{
      response: """
        {
          id: "file-abc123",
          object: "vector_store.file.deleted",
          deleted: true
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files/file-abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2" \\
            -X DELETE
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file_deleted = client.vector_stores.files.delete(
              file_id="file_id",
              vector_store_id="vector_store_id",
          )
          print(vector_store_file_deleted.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFileDeleted = await client.vectorStores.files.delete('file_id', {
            vector_store_id: 'vector_store_id',
          });
          
          console.log(vectorStoreFileDeleted.id);
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
            vectorStoreFileDeleted, err := client.VectorStores.Files.Delete(
              context.TODO(),
              "vector_store_id",
              "file_id",
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFileDeleted.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.files.FileDeleteParams;
          import com.openai.models.vectorstores.files.VectorStoreFileDeleted;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileDeleteParams params = FileDeleteParams.builder()
                      .vectorStoreId("vector_store_id")
                      .fileId("file_id")
                      .build();
                  VectorStoreFileDeleted vectorStoreFileDeleted = client.vectorStores().files().delete(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file_deleted = openai.vector_stores.files.delete("file_id", vector_store_id: "vector_store_id")
          
          puts(vector_store_file_deleted)
          """,
      },
    },
  }
)
@tag("Vector stores")
op deleteVectorStoreFile(
  /** The ID of the vector store that the file belongs to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file to delete. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): DeleteVectorStoreFileResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Retrieves a vector store file. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve vector store file")
@get
@route("/vector_stores/{vector_store_id}/files/{file_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve vector store file",
    group: "vector_stores",
    returns: "The [vector store file](https://platform.openai.com/docs/api-reference/vector-stores-files/file-object) object.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "vector_store.file",
          "created_at": 1699061776,
          "vector_store_id": "vs_abcd",
          "status": "completed",
          "last_error": null
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/vs_abc123/files/file-abc123 \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -H "OpenAI-Beta: assistants=v2"
          
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file = client.vector_stores.files.retrieve(
              file_id="file-abc123",
              vector_store_id="vs_abc123",
          )
          print(vector_store_file.id)
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFile = await client.vectorStores.files.retrieve('file-abc123', {
            vector_store_id: 'vs_abc123',
          });
          
          console.log(vectorStoreFile.id);
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
            vectorStoreFile, err := client.VectorStores.Files.Get(
              context.TODO(),
              "vs_abc123",
              "file-abc123",
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFile.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.models.vectorstores.files.FileRetrieveParams;
          import com.openai.models.vectorstores.files.VectorStoreFile;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileRetrieveParams params = FileRetrieveParams.builder()
                      .vectorStoreId("vs_abc123")
                      .fileId("file-abc123")
                      .build();
                  VectorStoreFile vectorStoreFile = client.vectorStores().files().retrieve(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file = openai.vector_stores.files.retrieve("file-abc123", vector_store_id: "vs_abc123")
          
          puts(vector_store_file)
          """,
      },
    },
  }
)
@tag("Vector stores")
op getVectorStoreFile(
  /** The ID of the vector store that the file belongs to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file being retrieved. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): VectorStoreFileObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Update attributes on a vector store file. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Update vector store file attributes")
@post
@route("/vector_stores/{vector_store_id}/files/{file_id}")
@extension(
  "x-oaiMeta",
  #{
    name: "Update vector store file attributes",
    group: "vector_stores",
    returns: "The updated [vector store file](https://platform.openai.com/docs/api-reference/vector-stores-files/file-object) object.",
    examples: #{
      response: """
        {
          "id": "file-abc123",
          "object": "vector_store.file",
          "usage_bytes": 1234,
          "created_at": 1699061776,
          "vector_store_id": "vs_abcd",
          "status": "completed",
          "last_error": null,
          "chunking_strategy": {...},
          "attributes": {"key1": "value1", "key2": 2}
        }
        
        """,
      request: #{
        curl: """
          curl https://api.openai.com/v1/vector_stores/{vector_store_id}/files/{file_id} \\
            -H "Authorization: Bearer $OPENAI_API_KEY" \\
            -H "Content-Type: application/json" \\
            -d '{"attributes": {"key1": "value1", "key2": 2}}'
          
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          const vectorStoreFile = await client.vectorStores.files.update('file-abc123', {
            vector_store_id: 'vs_abc123',
            attributes: { foo: 'string' },
          });
          
          console.log(vectorStoreFile.id);
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          vector_store_file = client.vector_stores.files.update(
              file_id="file-abc123",
              vector_store_id="vs_abc123",
              attributes={
                  "foo": "string"
              },
          )
          print(vector_store_file.id)
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
            vectorStoreFile, err := client.VectorStores.Files.Update(
              context.TODO(),
              "vs_abc123",
              "file-abc123",
              openai.VectorStoreFileUpdateParams{
                Attributes: map[string]openai.VectorStoreFileUpdateParamsAttributeUnion{
                "foo": openai.VectorStoreFileUpdateParamsAttributeUnion{
                  OfString: openai.String("string"),
                },
                },
              },
            )
            if err != nil {
              panic(err.Error())
            }
            fmt.Printf("%+v
          ", vectorStoreFile.ID)
          }
          
          """,
        java: """
          package com.openai.example;
          
          import com.openai.client.OpenAIClient;
          import com.openai.client.okhttp.OpenAIOkHttpClient;
          import com.openai.core.JsonValue;
          import com.openai.models.vectorstores.files.FileUpdateParams;
          import com.openai.models.vectorstores.files.VectorStoreFile;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileUpdateParams params = FileUpdateParams.builder()
                      .vectorStoreId("vs_abc123")
                      .fileId("file-abc123")
                      .attributes(FileUpdateParams.Attributes.builder()
                          .putAdditionalProperty("foo", JsonValue.from("string"))
                          .build())
                      .build();
                  VectorStoreFile vectorStoreFile = client.vectorStores().files().update(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          vector_store_file = openai.vector_stores.files.update(
            "file-abc123",
            vector_store_id: "vs_abc123",
            attributes: {foo: "string"}
          )
          
          puts(vector_store_file)
          """,
      },
    },
  }
)
@tag("Vector stores")
op updateVectorStoreFileAttributes(
  /** The ID of the vector store the file belongs to. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file to update attributes. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: UpdateVectorStoreFileAttributesRequest,
): VectorStoreFileObject | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Retrieve the parsed contents of a vector store file. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-names" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Retrieve vector store file content")
@get
@route("/vector_stores/{vector_store_id}/files/{file_id}/content")
@extension(
  "x-oaiMeta",
  #{
    name: "Retrieve vector store file content",
    group: "vector_stores",
    returns: "The parsed contents of the specified vector store file.",
    examples: #{
      response: """
        {
          "file_id": "file-abc123",
          "filename": "example.txt",
          "attributes": {"key": "value"},
          "content": [
            {"type": "text", "text": "..."},
            ...
          ]
        }
        
        """,
      request: #{
        curl: """
          curl \\
          https://api.openai.com/v1/vector_stores/vs_abc123/files/file-abc123/content \\
          -H "Authorization: Bearer $OPENAI_API_KEY"
          
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const fileContentResponse of client.vectorStores.files.content('file-abc123', {
            vector_store_id: 'vs_abc123',
          })) {
            console.log(fileContentResponse.text);
          }
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.vector_stores.files.content(
              file_id="file-abc123",
              vector_store_id="vs_abc123",
          )
          page = page.data[0]
          print(page.text)
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
            page, err := client.VectorStores.Files.Content(
              context.TODO(),
              "vs_abc123",
              "file-abc123",
            )
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
          import com.openai.models.vectorstores.files.FileContentPage;
          import com.openai.models.vectorstores.files.FileContentParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  FileContentParams params = FileContentParams.builder()
                      .vectorStoreId("vs_abc123")
                      .fileId("file-abc123")
                      .build();
                  FileContentPage page = client.vectorStores().files().content(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.vector_stores.files.content("file-abc123", vector_store_id: "vs_abc123")
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Vector stores")
op retrieveVectorStoreFileContent(
  /** The ID of the vector store. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  /** The ID of the file within the vector store. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  file_id: string,
): VectorStoreFileContentResponse | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};

/** Search a vector store for relevant chunks based on a query and file attributes filter. */
#suppress "@azure-tools/typespec-azure-core/response-schema-problem" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/use-standard-operations" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@summary("Search vector store")
@post
@route("/vector_stores/{vector_store_id}/search")
@extension(
  "x-oaiMeta",
  #{
    name: "Search vector store",
    group: "vector_stores",
    returns: "A page of search results from the vector store.",
    examples: #{
      response: """
        {
          "object": "vector_store.search_results.page",
          "search_query": "What is the return policy?",
          "data": [
            {
              "file_id": "file_123",
              "filename": "document.pdf",
              "score": 0.95,
              "attributes": {
                "author": "John Doe",
                "date": "2023-01-01"
              },
              "content": [
                {
                  "type": "text",
                  "text": "Relevant chunk"
                }
              ]
            },
            {
              "file_id": "file_456",
              "filename": "notes.txt",
              "score": 0.89,
              "attributes": {
                "author": "Jane Smith",
                "date": "2023-01-02"
              },
              "content": [
                {
                  "type": "text",
                  "text": "Sample text content from the vector store."
                }
              ]
            }
          ],
          "has_more": false,
          "next_page": null
        }
        
        """,
      request: #{
        curl: """
          curl -X POST \\
          https://api.openai.com/v1/vector_stores/vs_abc123/search \\
          -H "Authorization: Bearer $OPENAI_API_KEY" \\
          -H "Content-Type: application/json" \\
          -d '{"query": "What is the return policy?", "filters": {...}}'
          
          """,
        `node.js`: """
          import OpenAI from 'openai';
          
          const client = new OpenAI({
            apiKey: process.env['OPENAI_API_KEY'], // This is the default and can be omitted
          });
          
          // Automatically fetches more pages as needed.
          for await (const vectorStoreSearchResponse of client.vectorStores.search('vs_abc123', {
            query: 'string',
          })) {
            console.log(vectorStoreSearchResponse.file_id);
          }
          """,
        python: """
          import os
          from openai import OpenAI
          
          client = OpenAI(
              api_key=os.environ.get("OPENAI_API_KEY"),  # This is the default and can be omitted
          )
          page = client.vector_stores.search(
              vector_store_id="vs_abc123",
              query="string",
          )
          page = page.data[0]
          print(page.file_id)
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
            page, err := client.VectorStores.Search(
              context.TODO(),
              "vs_abc123",
              openai.VectorStoreSearchParams{
                Query: openai.VectorStoreSearchParamsQueryUnion{
                  OfString: openai.String("string"),
                },
              },
            )
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
          import com.openai.models.vectorstores.VectorStoreSearchPage;
          import com.openai.models.vectorstores.VectorStoreSearchParams;
          
          public final class Main {
              private Main() {}
          
              public static void main(String[] args) {
                  OpenAIClient client = OpenAIOkHttpClient.fromEnv();
          
                  VectorStoreSearchParams params = VectorStoreSearchParams.builder()
                      .vectorStoreId("vs_abc123")
                      .query("string")
                      .build();
                  VectorStoreSearchPage page = client.vectorStores().search(params);
              }
          }
          """,
        ruby: """
          require "openai"
          
          openai = OpenAI::Client.new(api_key: "My API Key")
          
          page = openai.vector_stores.search("vs_abc123", query: "string")
          
          puts(page)
          """,
      },
    },
  }
)
@tag("Vector stores")
op searchVectorStore(
  /** The ID of the vector store to search. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  @path
  vector_store_id: string,

  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  @body
  body: VectorStoreSearchRequest,
): VectorStoreSearchResultsPage | {
  @maxValue(499)
  @minValue(400)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
} | {
  @maxValue(599)
  @minValue(500)
  @statusCode
  statusCode: int32;

  @body
  body: ErrorResponse;
};
