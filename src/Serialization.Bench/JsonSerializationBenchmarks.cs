using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Mathematics;
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
    
    [GlobalSetup]
    public void Setup()
    {
        _smallPayload = SerializationHelper.CreateSampleFromFile<SmallPayload>("small-github-user.json");
        _mediumPayload = SerializationHelper.CreateSampleFromFile<MediumPayload>("medium-github-repo.json");
        _largePayload = SerializationHelper.CreateSampleFromFile<LargePayload>("large-nasa-comets.json");

        // _smallJson = System.Text.Json.JsonSerializer.Serialize(_smallPayload);
    }

    [Benchmark(Baseline = true)]
    public string SystemTextJson_Serialize_Small()
    {
        return System.Text.Json.JsonSerializer.Serialize(_smallPayload);
    }

    [Benchmark]
    public string Newtonsoft_Serialize_Small()
    {
        return JsonConvert.SerializeObject(_smallPayload);
    }
    
    [Benchmark]
    public string Jil_Serialize_Small()
    {
        return JSON.Serialize(_smallPayload);
    }
    
    [Benchmark]
    public string Utf8Json_Serialize_Small()
    {
        return Utf8Json.JsonSerializer.ToJsonString(_smallPayload);
    }
    
    [Benchmark]
    public string SpanJson_Serialize_Small()
    {
        return SpanJson.JsonSerializer.Generic.Utf16.Serialize(_smallPayload);
    }
    [Benchmark]
    public string SystemTextJson_Serialize_Medium()
    {
        return System.Text.Json.JsonSerializer.Serialize(_mediumPayload);
    }
    
    [Benchmark]
    public string Newtonsoft_Serialize_Medium()
    {
        return JsonConvert.SerializeObject(_mediumPayload);
    }
    
    [Benchmark]
    public string SystemTextJson_Serialize_Large()
    {
        return System.Text.Json.JsonSerializer.Serialize(_largePayload);
    }
    
    [Benchmark]
    public string Newtonsoft_Serialize_Large()
    {
        return JsonConvert.SerializeObject(_largePayload);
    }
    [Benchmark]
    public SmallPayload SystemTextJson_Deserialize_Small()
    {
        return System.Text.Json.JsonSerializer.Deserialize<SmallPayload>(_smallJson);
    }
}