using System.Text.Json.Serialization;

namespace Serialization.Bench.Models.Large;

public class LargePayload
{
    public Meta meta { get; set; }
    public object[][] data { get; set; }
}

public class Meta
{
    public View view { get; set; }
}

public class View
{
    public string id { get; set; }
    public string name { get; set; }
    public string assetType { get; set; }
    public string attribution { get; set; }
    public string attributionLink { get; set; }
    public int averageRating { get; set; }
    public string category { get; set; }
    public int createdAt { get; set; }
    public string description { get; set; }
    public bool diciBackend { get; set; }
    public string displayType { get; set; }
    public int downloadCount { get; set; }
    public bool hideFromCatalog { get; set; }
    public bool hideFromDataJson { get; set; }
    public int indexUpdatedAt { get; set; }
    public bool locked { get; set; }
    public bool newBackend { get; set; }
    public int numberOfComments { get; set; }
    public int oid { get; set; }
    public string provenance { get; set; }
    public bool publicationAppendEnabled { get; set; }
    public int publicationDate { get; set; }
    public int publicationGroup { get; set; }
    public string publicationStage { get; set; }
    public string rowClass { get; set; }
    public int rowsUpdatedAt { get; set; }
    public string rowsUpdatedBy { get; set; }
    public int tableId { get; set; }
    public int totalTimesRated { get; set; }
    public int viewCount { get; set; }
    public int viewLastModified { get; set; }
    public string viewType { get; set; }
    public Approvals[] approvals { get; set; }
    public ClientContext clientContext { get; set; }
    public Columns[] columns { get; set; }
    public Grants[] grants { get; set; }
    public Metadata metadata { get; set; }
    public Owner owner { get; set; }
    public Query query { get; set; }
    public Ratings ratings { get; set; }
    public string[] rights { get; set; }
    public TableAuthor tableAuthor { get; set; }
    public string[] tags { get; set; }
    public string[] flags { get; set; }
}

public class Approvals
{
    public int reviewedAt { get; set; }
    public bool reviewedAutomatically { get; set; }
    public string state { get; set; }
    public int submissionId { get; set; }
    public string submissionObject { get; set; }
    public string submissionOutcome { get; set; }
    public int submittedAt { get; set; }
    public string targetAudience { get; set; }
    public int workflowId { get; set; }
    public SubmissionDetails submissionDetails { get; set; }
    public SubmissionOutcomeApplication submissionOutcomeApplication { get; set; }
    public Submitter submitter { get; set; }
}

public class SubmissionDetails
{
    public string permissionType { get; set; }
}

public class SubmissionOutcomeApplication
{
    public int failureCount { get; set; }
    public string status { get; set; }
}

public class Submitter
{
    public string id { get; set; }
    public string displayName { get; set; }
}

public class ClientContext
{
    public object[] clientContextVariables { get; set; }
    public InheritedVariables inheritedVariables { get; set; }
}

public class InheritedVariables
{

}

public class Columns
{
    public int id { get; set; }
    public string name { get; set; }
    public string dataTypeName { get; set; }
    public string fieldName { get; set; }
    public int position { get; set; }
    public string renderTypeName { get; set; }
    public Format format { get; set; }
    public string[] flags { get; set; }
    public int tableColumnId { get; set; }
    public int width { get; set; }
    public CachedContents cachedContents { get; set; }
}

public class Format
{
    public string align { get; set; }
}

public class CachedContents
{
    public string non_null { get; set; }
    public string largest { get; set; }
    [JsonPropertyName("null")]
    public string is_null { get; set; }
    public Top[] top { get; set; }
    public string smallest { get; set; }
    public string count { get; set; }
    public string cardinality { get; set; }
    public string average { get; set; }
    public string sum { get; set; }
}

public class Top
{
    public string item { get; set; }
    public string count { get; set; }
}

public class Grants
{
    public bool inherited { get; set; }
    public string type { get; set; }
    public string[] flags { get; set; }
}

public class Metadata
{
    public string rdfSubject { get; set; }
    public string rdfClass { get; set; }
    public Custom_fields custom_fields { get; set; }
    public string rowIdentifier { get; set; }
    public string[] availableDisplayTypes { get; set; }
    public string rowLabel { get; set; }
    public RenderTypeConfig renderTypeConfig { get; set; }
}

public class Custom_fields
{
    public Common_Core Common_Core { get; set; }
}

public class Common_Core
{
    public string Program_Code { get; set; }
    public string[] Bureau_Code { get; set; }
}

public class RenderTypeConfig
{
    public Visible visible { get; set; }
}

public class Visible
{
    public bool table { get; set; }
}

public class Owner
{
    public string id { get; set; }
    public int disabledAt { get; set; }
    public string displayName { get; set; }
    public string profileImageUrlLarge { get; set; }
    public string profileImageUrlMedium { get; set; }
    public string profileImageUrlSmall { get; set; }
    public string screenName { get; set; }
    public string type { get; set; }
    public string[] flags { get; set; }
}

public class Query
{

}

public class Ratings
{
    public int rating { get; set; }
}

public class TableAuthor
{
    public string id { get; set; }
    public int disabledAt { get; set; }
    public string displayName { get; set; }
    public string profileImageUrlLarge { get; set; }
    public string profileImageUrlMedium { get; set; }
    public string profileImageUrlSmall { get; set; }
    public string screenName { get; set; }
    public string type { get; set; }
    public string[] flags { get; set; }
}

