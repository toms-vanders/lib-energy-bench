using System.Runtime.CompilerServices;
using System.Text.Json;

namespace Serialization.Bench.Helpers;

public static class SerializationHelper
{
    public static string WhereAmI([CallerFilePath] string callerFilePath = "") 
        => callerFilePath;
    
    public static string FilePath(string fileName)
        => Path.Combine(Path.GetDirectoryName(WhereAmI()), "..", "data", "json", fileName);
    
    public static string ResultPath()
        => Path.Combine(Path.GetDirectoryName(WhereAmI()), "..", "BenchmarkArtifacts");

    public static (T Payload, string Json) LoadSample<T>(string fileName)
    {
        var filePath = FilePath(fileName);
        var json     = File.ReadAllText(filePath);
    
        var payload = System.Text.Json.JsonSerializer.Deserialize<T>(json)
                      ?? throw new InvalidOperationException("Failed to parse sample JSON.");

        return (payload, json);
    }
}