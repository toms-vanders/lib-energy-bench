using System.Text.Json.Nodes;

namespace UnitTests;

public static class JsonTestHelpers
{
    /// <summary>
    /// Normalize JSON through System.Text.Json so we compare models
    /// in a common representation.
    /// </summary>
    public static T NormalizeFromJson<T>(string json) =>
        System.Text.Json.JsonSerializer.Deserialize<T>(json)
        ?? throw new InvalidOperationException("Failed to deserialize JSON with System.Text.Json.");

    /// <summary>
    /// Asserts structural JSON equality by serializing both models with System.Text.Json
    /// and comparing the resulting JSON trees.
    /// </summary>
    public static void AssertStructuralEqual<T>(T expected, T actual)
    {
        var expectedJson = System.Text.Json.JsonSerializer.Serialize(expected);
        var actualJson   = System.Text.Json.JsonSerializer.Serialize(actual);

        var expectedNode = JsonNode.Parse(expectedJson);
        var actualNode   = JsonNode.Parse(actualJson);

        Assert.True(JsonNode.DeepEquals(expectedNode, actualNode),
            $"JSON not equivalent.\nExpected: {expectedJson}\nActual:   {actualJson}");
    }}