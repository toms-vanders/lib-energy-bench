namespace Serialization.Bench.Models;

public class CommitItem
{
    public string url { get; set; }
    public string sha { get; set; }
    public string node_id { get; set; }
    public string html_url { get; set; }
    public string comments_url { get; set; }
    public CommitInfo commit { get; set; }
    public SimpleUser? author { get; set; }
    public SimpleUser? committer { get; set; }
    public Parent[] parents { get; set; }
    public Stats? stats { get; set; }
    public DiffEntry[]? files { get; set; }
}

public class CommitInfo
{
    public string url { get; set; }

    public GitUser? author { get; set; }
    public GitUser? committer { get; set; }

    public string message { get; set; }
    public int comment_count { get; set; }

    public Tree tree { get; set; }

    public Verification verification { get; set; }
}

public class GitUser
{
    public string name { get; set; }
    public string email { get; set; }
    public DateTimeOffset date { get; set; }
}

public class Tree
{
    public string sha { get; set; }
    public string url { get; set; }
}

public class Verification
{
    public bool verified { get; set; }
    public string reason { get; set; }
    public string? payload { get; set; }
    public string? signature { get; set; }
    public string? verified_at { get; set; }
}

public class SimpleUser
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

public class Parent
{
    public string sha { get; set; }
    public string url { get; set; }
    public string? html_url { get; set; }
}

public class Stats
{
    public int additions { get; set; }
    public int deletions { get; set; }
    public int total { get; set; }
}

public class DiffEntry
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