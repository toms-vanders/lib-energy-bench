using BenchmarkDotNet.Attributes;
using Jil;
using Newtonsoft.Json;
using Serialization.Bench.Helpers;
using Serialization.Bench.Models;
using Serialization.Bench.Models.Large;

namespace Serialization.Bench;

// [DisassemblyDiagnoser]
[Config(typeof(BenchConfig))]
public class JsonSerializationBenchmarks
{
    private CommunityProfile _smallPayload;
    private CommitItem _mediumPayload;
    private CommitCompare _largePayload;
    private string _smallJson;
    private string _mediumJson;
    private string _largeJson;
    
    [GlobalSetup]
    public void Setup()
    {
        _smallPayload = SerializationHelper.CreateSampleFromFile<CommunityProfile>("small-profile.json");
        _mediumPayload = SerializationHelper.CreateSampleFromFile<CommitItem>("medium-commit.json");
        _largePayload = SerializationHelper.CreateSampleFromFile<CommitCompare>("large-compare.json");

        _smallJson = System.Text.Json.JsonSerializer.Serialize(_smallPayload);
        _mediumJson = System.Text.Json.JsonSerializer.Serialize(_mediumPayload);
        _largeJson  = System.Text.Json.JsonSerializer.Serialize(_largePayload);
    }

    // ===== Serialize · Small =====
    [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Small")]
    public string STJ_Serialize_Small() =>
        System.Text.Json.JsonSerializer.Serialize(_smallPayload);

    [Benchmark, BenchmarkCategory("Serialize", "Small")]
    public string Newtonsoft_Serialize_Small() =>
        JsonConvert.SerializeObject(_smallPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Small")]
    public string Jil_Serialize_Small() =>
        JSON.Serialize(_smallPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Small")]
    public string Utf8Json_Serialize_Small() =>
        Utf8Json.JsonSerializer.ToJsonString(_smallPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Small")]
    public string SpanJson_Serialize_Small() =>
        SpanJson.JsonSerializer.Generic.Utf16.Serialize(_smallPayload);
    
    // ===== Serialize · Medium =====
    [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Medium")]
    public string STJ_Serialize_Medium() =>
        System.Text.Json.JsonSerializer.Serialize(_mediumPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    public string Newtonsoft_Serialize_Medium() =>
        JsonConvert.SerializeObject(_mediumPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    public string Jil_Serialize_Medium() =>
        JSON.Serialize(_mediumPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    public string Utf8Json_Serialize_Medium() =>
        Utf8Json.JsonSerializer.ToJsonString(_mediumPayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    public string SpanJson_Serialize_Medium() =>
        SpanJson.JsonSerializer.Generic.Utf16.Serialize(_mediumPayload);
    
    // ===== Serialize · Large =====
    [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Large")]
    public string STJ_Serialize_Large() =>
        System.Text.Json.JsonSerializer.Serialize(_largePayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Large")]
    public string Newtonsoft_Serialize_Large() =>
        JsonConvert.SerializeObject(_largePayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Large")]
    public string Jil_Serialize_Large() =>
        JSON.Serialize(_largePayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Large")]
    public string Utf8Json_Serialize_Large() =>
        Utf8Json.JsonSerializer.ToJsonString(_largePayload);
    
    [Benchmark, BenchmarkCategory("Serialize", "Large")]
    public string SpanJson_Serialize_Large() =>
        SpanJson.JsonSerializer.Generic.Utf16.Serialize(_largePayload);
    
        // ===== Deserialize · Small =====
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Small")]
    public CommunityProfile STJ_Deserialize_Small() =>
        System.Text.Json.JsonSerializer.Deserialize<CommunityProfile>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public CommunityProfile Newtonsoft_Deserialize_Small() =>
        JsonConvert.DeserializeObject<CommunityProfile>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public CommunityProfile Jil_Deserialize_Small() =>
        JSON.Deserialize<CommunityProfile>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public CommunityProfile Utf8Json_Deserialize_Small() =>
        Utf8Json.JsonSerializer.Deserialize<CommunityProfile>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public CommunityProfile SpanJson_Deserialize_Small() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommunityProfile>(_smallJson);
    
    // ===== Deserialize · Medium =====
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Medium")]
    public CommitItem STJ_Deserialize_Medium() =>
        System.Text.Json.JsonSerializer.Deserialize<CommitItem>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public CommitItem Newtonsoft_Deserialize_Medium() =>
        JsonConvert.DeserializeObject<CommitItem>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public CommitItem Jil_Deserialize_Medium() =>
        JSON.Deserialize<CommitItem>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public CommitItem Utf8Json_Deserialize_Medium() =>
        Utf8Json.JsonSerializer.Deserialize<CommitItem>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public CommitItem SpanJson_Deserialize_Medium() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitItem>(_mediumJson);
    
    // ===== Deserialize · Large =====
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Large")]
    public CommitCompare STJ_Deserialize_Large() =>
        System.Text.Json.JsonSerializer.Deserialize<CommitCompare>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public CommitCompare Newtonsoft_Deserialize_Large() =>
        JsonConvert.DeserializeObject<CommitCompare>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public CommitCompare Jil_Deserialize_Large() =>
        JSON.Deserialize<CommitCompare>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public CommitCompare Utf8Json_Deserialize_Large() =>
        Utf8Json.JsonSerializer.Deserialize<CommitCompare>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public CommitCompare SpanJson_Deserialize_Large() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitCompare>(_largeJson);
}