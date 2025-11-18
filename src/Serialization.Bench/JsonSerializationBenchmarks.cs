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
    private SmallPayload _smallPayload;
    private MediumPayload _mediumPayload;
    private LargePayload _largePayload;
    private string _smallJson;
    private string _mediumJson;
    private string _largeJson;
    
    [GlobalSetup]
    public void Setup()
    {
        _smallPayload = SerializationHelper.CreateSampleFromFile<SmallPayload>("small-github-user.json");
        _mediumPayload = SerializationHelper.CreateSampleFromFile<MediumPayload>("medium-github-repo.json");
        _largePayload = SerializationHelper.CreateSampleFromFile<LargePayload>("large-nasa-comets.json");

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
    public SmallPayload STJ_Deserialize_Small() =>
        System.Text.Json.JsonSerializer.Deserialize<SmallPayload>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public SmallPayload Newtonsoft_Deserialize_Small() =>
        JsonConvert.DeserializeObject<SmallPayload>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public SmallPayload Jil_Deserialize_Small() =>
        JSON.Deserialize<SmallPayload>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public SmallPayload Utf8Json_Deserialize_Small() =>
        Utf8Json.JsonSerializer.Deserialize<SmallPayload>(_smallJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    public SmallPayload SpanJson_Deserialize_Small() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<SmallPayload>(_smallJson);
    
    // ===== Deserialize · Medium =====
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Medium")]
    public MediumPayload STJ_Deserialize_Medium() =>
        System.Text.Json.JsonSerializer.Deserialize<MediumPayload>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public MediumPayload Newtonsoft_Deserialize_Medium() =>
        JsonConvert.DeserializeObject<MediumPayload>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public MediumPayload Jil_Deserialize_Medium() =>
        JSON.Deserialize<MediumPayload>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public MediumPayload Utf8Json_Deserialize_Medium() =>
        Utf8Json.JsonSerializer.Deserialize<MediumPayload>(_mediumJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    public MediumPayload SpanJson_Deserialize_Medium() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<MediumPayload>(_mediumJson);
    
    // ===== Deserialize · Large =====
    [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Large")]
    public LargePayload STJ_Deserialize_Large() =>
        System.Text.Json.JsonSerializer.Deserialize<LargePayload>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public LargePayload Newtonsoft_Deserialize_Large() =>
        JsonConvert.DeserializeObject<LargePayload>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public LargePayload Jil_Deserialize_Large() =>
        JSON.Deserialize<LargePayload>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public LargePayload Utf8Json_Deserialize_Large() =>
        Utf8Json.JsonSerializer.Deserialize<LargePayload>(_largeJson);
    
    [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    public LargePayload SpanJson_Deserialize_Large() =>
        SpanJson.JsonSerializer.Generic.Utf16.Deserialize<LargePayload>(_largeJson);
}