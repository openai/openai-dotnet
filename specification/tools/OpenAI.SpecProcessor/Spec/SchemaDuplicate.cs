namespace OpenAI.SpecProcessor.Spec;

/// <summary> Represents a pair of schemas that are likely duplicates based on structural similarity. </summary>
/// <param name="SchemaA"> The first schema name. </param>
/// <param name="SchemaB"> The second schema name. </param>
/// <param name="Similarity"> The Jaccard similarity score (0.0–1.0). </param>
/// <param name="SharedProperties"> The property names shared by both schemas. </param>
public record SchemaDuplicate(
    string SchemaA,
    string SchemaB,
    double Similarity,
    IReadOnlyList<string> SharedProperties);
