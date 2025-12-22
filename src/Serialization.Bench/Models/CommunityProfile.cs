namespace Serialization.Bench.Models;

public class CommunityProfile
{
    public int health_percentage { get; set; }
    public string description { get; set; }
    public string documentation { get; set; }
    public CommunityFiles files { get; set; }
    public string? updated_at { get; set; }
    public bool content_reports_enabled { get; set; }
}

public class CommunityFiles
{
    public CodeOfConductSimple? code_of_conduct { get; set; }
    public CommunityHealthFile? code_of_conduct_file { get; set; }
    public LicenseSimple? license { get; set; }
    public CommunityHealthFile? contributing { get; set; }
    public CommunityHealthFile? readme { get; set; }
    public CommunityHealthFile? issue_template { get; set; }
    public CommunityHealthFile? pull_request_template { get; set; }
}

public class CodeOfConductSimple
{
    public string url { get; set; }
    public string key { get; set; }
    public string name { get; set; }
    public string html_url { get; set; }
}

public class CommunityHealthFile
{
    public string url { get; set; }
    public string html_url { get; set; }
}

public class LicenseSimple
{
    public string key { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public string spdx_id { get; set; }
    public string node_id { get; set; }
    public string html_url { get; set; }
}