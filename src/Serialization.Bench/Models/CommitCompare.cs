namespace Serialization.Bench.Models;

public class CommitCompare
{
    public string url { get; set; }
    public string html_url { get; set; }
    public string permalink_url { get; set; }
    public string diff_url { get; set; }
    public string patch_url { get; set; }

    public CompareCommit base_commit { get; set; }
    public CompareCommit merge_base_commit { get; set; }

    public string status { get; set; }
    public int ahead_by { get; set; }
    public int behind_by { get; set; }
    public int total_commits { get; set; }

    public CompareCommit[] commits { get; set; }

    public CompareDiffEntry[]? files { get; set; }
}

public class CompareCommit
{
    public string sha { get; set; }
    public string node_id { get; set; }
    public string url { get; set; }
    public string html_url { get; set; }
    public string comments_url { get; set; }

    public CompareCommitInfo commit { get; set; }

    public CompareSimpleUser? author { get; set; }
    public CompareSimpleUser? committer { get; set; }

    public CompareParent[] parents { get; set; }

    public CompareStats? stats { get; set; }

    public CompareDiffEntry[]? files { get; set; }
}

public class CompareCommitInfo
{
    public string url { get; set; }
    public CompareGitUser? author { get; set; }
    public CompareGitUser? committer { get; set; }

    public string message { get; set; }
    public int comment_count { get; set; }

    public CompareTree tree { get; set; }
    public CompareVerification verification { get; set; }
}

public class CompareGitUser
{
    public string name { get; set; }
    public string email { get; set; }
    public DateTimeOffset date { get; set; }
}

public class CompareTree
{
    public string sha { get; set; }
    public string url { get; set; }
}

public class CompareVerification
{
    public bool verified { get; set; }
    public string reason { get; set; }

    public string? payload { get; set; }
    public string? signature { get; set; }

    public DateTimeOffset? verified_at { get; set; }
}

public class CompareSimpleUser
{
    public string? name { get; set; }
    public string? email { get; set; }

    public string login { get; set; }
    public long id { get; set; }
    public string node_id { get; set; }

    public string avatar_url { get; set; }
    public string? gravatar_id { get; set; }

    public string url { get; set; }
    public string html_url { get; set; }

    public string followers_url { get; set; }
    public string following_url { get; set; }
    public string gists_url { get; set; }
    public string starred_url { get; set; }
    public string subscriptions_url { get; set; }
    public string organizations_url { get; set; }
    public string repos_url { get; set; }
    public string events_url { get; set; }
    public string received_events_url { get; set; }

    public string type { get; set; }
    public bool site_admin { get; set; }

    public string? starred_at { get; set; }
    public string? user_view_type { get; set; }
}

public class CompareParent
{
    public string sha { get; set; }
    public string url { get; set; }
    public string? html_url { get; set; }
}

public class CompareStats
{
    public int additions { get; set; }
    public int deletions { get; set; }
    public int total { get; set; }
}

public class CompareDiffEntry
{
    public string? sha { get; set; }
    public string filename { get; set; }
    public string status { get; set; }

    public int additions { get; set; }
    public int deletions { get; set; }
    public int changes { get; set; }

    public string blob_url { get; set; }
    public string raw_url { get; set; }
    public string contents_url { get; set; }

    public string? patch { get; set; }
    public string? previous_filename { get; set; }
}