namespace OpenAI.SpecProcessor.Spec;

/// <summary>
/// Defines all feature areas, exclusion rules, and metadata fields to strip.
/// </summary>
/// <remarks>
/// Area names and output file names follow the taxonomy the repository already uses for spec
/// ingestion, which is the folder set under <c>specification/base/typespec</c>, so that the diff
/// report speaks the same language as the code and the TypeSpec layout. Note that the
/// <c>.github/skills/ingesting-spec/file-locations.md</c> table is a stale view of that taxonomy;
/// the folders on disk are authoritative.
/// </remarks>
public static class FeatureAreaConfig
{
    /// <summary> Paths removed entirely from the spec before any processing. </summary>
    public static readonly string[] ExcludedPathPrefixes = ["/chatkit"];

    /// <summary> Exact paths excluded from the spec before any processing. </summary>
    public static readonly string[] ExcludedExactPaths =
    [
        "/completions",
        "/realtime/sessions",
        "/realtime/transcription_sessions"
    ];

    /// <summary> Tags whose associated operations are excluded from the spec. </summary>
    public static readonly string[] ExcludedTags = ["Completions"];

    /// <summary> Top-level metadata keys stripped from the entire spec tree. </summary>
    public static readonly string[] MetadataKeysToStrip = ["x-oaiMeta", "x-oaiTypeLabel"];

    /// <summary> The feature areas the spec is split across. </summary>
    public static readonly FeatureArea[] All =
    [
        new()
        {
            Name = "Responses", OutputFile = "responses.yml",
            Tags = ["Responses"],
            PathPrefixes = ["/responses"],
            ExplicitPaths = ["/responses/compact", "/responses/input_tokens"]
        },
        new()
        {
            Name = "Conversations", OutputFile = "conversations.yml",
            Tags = ["Conversations"],
            PathPrefixes = ["/conversations"]
        },
        new()
        {
            Name = "Chat", OutputFile = "chat.yml",
            Tags = ["Chat"],
            PathPrefixes = ["/chat"]
        },
        new()
        {
            Name = "Audio", OutputFile = "audio.yml",
            Tags = ["Audio"],
            PathPrefixes = ["/audio"]
        },
        new()
        {
            Name = "Videos", OutputFile = "videos.yml",
            Tags = ["Videos"],
            PathPrefixes = ["/videos"]
        },
        new()
        {
            Name = "Images", OutputFile = "images.yml",
            Tags = ["Images"],
            PathPrefixes = ["/images"]
        },
        new()
        {
            Name = "Embeddings", OutputFile = "embeddings.yml",
            Tags = ["Embeddings"],
            PathPrefixes = ["/embeddings"]
        },
        new()
        {
            Name = "Evals", OutputFile = "evals.yml",
            Tags = ["Evals"],
            PathPrefixes = ["/evals"]
        },
        new()
        {
            Name = "Graders", OutputFile = "graders.yml",
            Tags = [],
            PathPrefixes = ["/fine_tuning/alpha/graders"]
        },
        new()
        {
            Name = "Fine Tuning", OutputFile = "fine-tuning.yml",
            Tags = ["Fine-tuning"],
            PathPrefixes = ["/fine_tuning"],
            ExcludedPathPrefixes = ["/fine_tuning/alpha/graders"]
        },
        new()
        {
            Name = "Batch", OutputFile = "batch.yml",
            Tags = ["Batch"],
            PathPrefixes = ["/batches"]
        },
        new()
        {
            Name = "Files", OutputFile = "files.yml",
            Tags = ["Files"],
            PathPrefixes = ["/files"]
        },
        new()
        {
            Name = "Uploads", OutputFile = "uploads.yml",
            Tags = ["Uploads"],
            PathPrefixes = ["/uploads"]
        },
        new()
        {
            Name = "Models", OutputFile = "models.yml",
            Tags = ["Models"],
            PathPrefixes = ["/models"]
        },
        new()
        {
            Name = "Moderations", OutputFile = "moderations.yml",
            Tags = ["Moderations"],
            PathPrefixes = ["/moderations"]
        },
        new()
        {
            Name = "Vector Stores", OutputFile = "vector-stores.yml",
            Tags = ["Vector stores"],
            PathPrefixes = ["/vector_stores"]
        },
        new()
        {
            Name = "Containers", OutputFile = "containers.yml",
            Tags = [],
            PathPrefixes = ["/containers"]
        },
        new()
        {
            Name = "Skills", OutputFile = "skills.yml",
            Tags = ["Skills"],
            PathPrefixes = ["/skills"]
        },
        new()
        {
            Name = "Realtime", OutputFile = "realtime.yml",
            Tags = ["Realtime"],
            PathPrefixes = ["/realtime/calls", "/realtime/client_secrets"]
        },
        new()
        {
            Name = "Assistants", OutputFile = "assistants.yml",
            Tags = ["Assistants"],
            PathPrefixes = ["/assistants"],
            ExcludedPathPrefixes = ["/threads"]
        },
        new()
        {
            Name = "Messages", OutputFile = "messages.yml",
            Tags = [],
            PathPrefixes = ["/threads/{thread_id}/messages"]
        },
        new()
        {
            Name = "Runs", OutputFile = "runs.yml",
            Tags = [],
            PathPrefixes = ["/threads/runs", "/threads/{thread_id}/runs"]
        },
        new()
        {
            Name = "Threads", OutputFile = "threads.yml",
            Tags = [],
            PathPrefixes = ["/threads"],
            ExcludedPathPrefixes =
            [
                "/threads/runs",
                "/threads/{thread_id}/messages",
                "/threads/{thread_id}/runs"
            ]
        },
        new()
        {
            Name = "Administration", OutputFile = "administration.yml",
            Tags =
            [
                "Audit Logs", "Invites", "Projects", "Users", "Groups", "Roles",
                "Certificates", "Usage",
                "Group organization role assignments", "Group users",
                "Project groups", "Project group role assignments",
                "Project user role assignments",
                "User organization role assignments"
            ],
            PathPrefixes = ["/organization", "/projects"],
            ExplicitPaths = ["/organization/admin_api_keys"]
        }
    ];

    /// <summary> Determines whether a path should be excluded from the spec entirely. </summary>
    /// <param name="path"> The path to check for exclusion. </param>
    /// <returns> <c>true</c> if the path is excluded; otherwise, <c>false</c>. </returns>
    public static bool IsExcludedPath(string path)
    {
        foreach (var prefix in ExcludedPathPrefixes)
        {
            if (path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        foreach (var exact in ExcludedExactPaths)
        {
            if (path.Equals(exact, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Finds the feature area that owns a given path and optional tag set.
    /// Returns null if unassigned.
    /// </summary>
    /// <param name="path"> The API path to match. </param>
    /// <param name="tags"> An optional set of tags associated with the path. </param>
    /// <returns> The matching <see cref="FeatureArea"/>, or <c>null</c> if unassigned. </returns>
    public static FeatureArea? FindFeatureArea(string path, IReadOnlyCollection<string>? tags)
    {
        // Tag-based matching takes priority.

        if (tags is { Count: > 0 })
        {
            foreach (var feature in All)
            {
                // A feature that disclaims the path does not get it, however it was tagged.

                if (IsPathExcludedFromFeature(feature, path))
                {
                    continue;
                }

                foreach (var tag in tags)
                {
                    if (feature.Tags.Contains(tag, StringComparer.OrdinalIgnoreCase))
                    {
                        return feature;
                    }
                }
            }
        }

        // Explicit path matching is checked next.

        foreach (var feature in All)
        {
            foreach (var explicitPath in feature.ExplicitPaths)
            {
                if (path.Equals(explicitPath, StringComparison.OrdinalIgnoreCase))
                {
                    return feature;
                }
            }
        }

        // Path prefix matching with exclusion check is the final fallback.

        foreach (var feature in All)
        {
            foreach (var prefix in feature.PathPrefixes)
            {
                if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                if (!IsPathExcludedFromFeature(feature, path))
                {
                    return feature;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determines whether a feature area explicitly disclaims a path.
    /// </summary>
    /// <param name="feature"> The feature area to check. </param>
    /// <param name="path"> The API path to test against the area's exclusions. </param>
    /// <returns> <c>true</c> if the area excludes the path; otherwise, <c>false</c>. </returns>
    private static bool IsPathExcludedFromFeature(FeatureArea feature, string path)
    {
        foreach (var excluded in feature.ExcludedPathPrefixes)
        {
            if (path.StartsWith(excluded, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
