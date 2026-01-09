import "@typespec/openapi";
using TypeSpec.OpenAPI;
namespace OpenAI;
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateModerationRequest {
  /**Input (or inputs) to classify. Can be a single string, an array of strings, or
  an array of multi-modal input objects similar to other models.*/
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-autorest/union-unsupported" "Auto-suppressed warnings non-applicable rules during import."
  input: string | string[] | CreateModerationRequestInput[];

  /**The content moderation model you would like to use. Learn more in
  [the moderation guide](https://platform.openai.com/docs/guides/moderation), and learn about
  available models [here](https://platform.openai.com/docs/models#moderation).*/
  #suppress "@azure-tools/typespec-azure-core/no-unnamed-union" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-oaiTypeLabel", "string")
  `model`?:
    | string
    | "omni-moderation-latest"
    | "omni-moderation-2024-09-26"
    | "text-moderation-latest"
    | "text-moderation-stable";
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
@discriminator("type")
model CreateModerationRequestInput {
  type: CreateModerationRequestInputType;
}

/** An object describing an image to classify. */
#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
model ModerationImageURLInput extends CreateModerationRequestInput {
  /** Always `image_url`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "image_url";

  /** Contains either an image URL or a data URL for a base64 encoded image. */
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  image_url: ModerationImageURLInputImageUrl;
}

#suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model ModerationImageURLInputImageUrl {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  url: url;
}

/** An object describing text to classify. */
model ModerationTextInput extends CreateModerationRequestInput {
  /** Always `text`. */
  #suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
  @extension("x-stainless-const", true)
  type: "text";

  /** A string of text to classify. */
  text: string;
}

/** Represents if a given text input is potentially harmful. */
#suppress "@azure-tools/typespec-azure-core/no-openapi" "Auto-suppressed warnings non-applicable rules during import."
@extension(
  "x-oaiMeta",
  #{
    name: "The moderation object",
    example: """
      {
        "id": "modr-0d9740456c391e43c445bf0f010940c7",
        "model": "omni-moderation-latest",
        "results": [
          {
            "flagged": true,
            "categories": {
              "harassment": true,
              "harassment/threatening": true,
              "sexual": false,
              "hate": false,
              "hate/threatening": false,
              "illicit": false,
              "illicit/violent": false,
              "self-harm/intent": false,
              "self-harm/instructions": false,
              "self-harm": false,
              "sexual/minors": false,
              "violence": true,
              "violence/graphic": true
            },
            "category_scores": {
              "harassment": 0.8189693396524255,
              "harassment/threatening": 0.804985420696006,
              "sexual": 1.573112165348997e-6,
              "hate": 0.007562942636942845,
              "hate/threatening": 0.004208854591835476,
              "illicit": 0.030535955153511665,
              "illicit/violent": 0.008925306722380033,
              "self-harm/intent": 0.00023023930975076432,
              "self-harm/instructions": 0.0002293869201073356,
              "self-harm": 0.012598046106750154,
              "sexual/minors": 2.212566909570261e-8,
              "violence": 0.9999992735124786,
              "violence/graphic": 0.843064871157054
            },
            "category_applied_input_types": {
              "harassment": [
                "text"
              ],
              "harassment/threatening": [
                "text"
              ],
              "sexual": [
                "text",
                "image"
              ],
              "hate": [
                "text"
              ],
              "hate/threatening": [
                "text"
              ],
              "illicit": [
                "text"
              ],
              "illicit/violent": [
                "text"
              ],
              "self-harm/intent": [
                "text",
                "image"
              ],
              "self-harm/instructions": [
                "text",
                "image"
              ],
              "self-harm": [
                "text",
                "image"
              ],
              "sexual/minors": [
                "text"
              ],
              "violence": [
                "text",
                "image"
              ],
              "violence/graphic": [
                "text",
                "image"
              ]
            }
          }
        ]
      }
      
      """,
  }
)
model CreateModerationResponse {
  /** The unique identifier for the moderation request. */
  id: string;

  /** The model used to generate the moderation results. */
  `model`: string;

  /** A list of moderation objects. */
  results: CreateModerationResponseResults[];
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateModerationResponseResults {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  flagged: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  categories: CreateModerationResponseResultsCategories;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  category_scores: CreateModerationResponseResultsCategoryScores;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  category_applied_input_types: CreateModerationResponseResultsCategoryAppliedInputTypes;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateModerationResponseResultsCategories {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  hate: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `hate/threatening`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  harassment: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `harassment/threatening`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  illicit: boolean | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/no-nullable" "Auto-suppressed warnings non-applicable rules during import."
  `illicit/violent`: boolean | null;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/intent`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/instructions`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  sexual: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `sexual/minors`: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  violence: boolean;
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `violence/graphic`: boolean;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateModerationResponseResultsCategoryScores {
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  hate: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `hate/threatening`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  harassment: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `harassment/threatening`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  illicit: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `illicit/violent`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/intent`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/instructions`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  sexual: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `sexual/minors`: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  violence: numeric;
  #suppress "@azure-tools/typespec-azure-core/no-generic-numeric" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `violence/graphic`: numeric;
}

#suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
model CreateModerationResponseResultsCategoryAppliedInputTypes {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  hate: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `hate/threatening`: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  harassment: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `harassment/threatening`: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  illicit: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `illicit/violent`: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm`: ModerationCategoryInputType[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/intent`: ModerationCategoryInputType[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `self-harm/instructions`: ModerationCategoryInputType[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  sexual: ModerationCategoryInputType[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `sexual/minors`: ModerationCategoryInputType.text[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  violence: ModerationCategoryInputType[];
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  #suppress "@azure-tools/typespec-azure-core/casing-style" "Auto-suppressed warnings non-applicable rules during import."
  `violence/graphic`: ModerationCategoryInputType[];
}

/** The applied input type(s) for the category. */
#suppress "@azure-tools/typespec-azure-core/no-enum" "Auto-suppressed warnings non-applicable rules during import."
enum ModerationCategoryInputType {
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  text,
  #suppress "@azure-tools/typespec-azure-core/documentation-required" "Auto-suppressed warnings non-applicable rules during import."
  image,
}

union CreateModerationRequestInputType {
  string,
  "image_url",
  "text",
}
