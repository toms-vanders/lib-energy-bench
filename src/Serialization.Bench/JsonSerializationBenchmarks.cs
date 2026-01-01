using BenchmarkDotNet.Attributes;
using Jil;
using Newtonsoft.Json;
using Serialization.Bench.Helpers;
using Serialization.Bench.Models;

namespace Serialization.Bench;

[Config(typeof(BenchConfig))]
public class JsonSerializationBenchmarks
{
    private CommunityProfile _smallObject;
    private CommitItem _mediumObject;
    private CommitCompare _largeObject;
    private string _smallJson;
    private string _mediumJson;
    private string _largeJson;
    
    [GlobalSetup]
    public void Setup()
    {
        var small = SerializationHelper.LoadSample<CommunityProfile>("small-profile.json");
        var medium = SerializationHelper.LoadSample<CommitItem>("medium-commit.json");
        var large = SerializationHelper.LoadSample<CommitCompare>("large-compare.json");

        _smallObject = small.Payload;
        _mediumObject = medium.Payload;
        _largeObject = large.Payload;

        _smallJson = small.Json;
        _mediumJson = medium.Json;
        _largeJson = large.Json;
    }

    #region SerializeSmall

    [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Small")]
    public string STJ_Serialize_Small() =>
        System.Text.Json.JsonSerializer.Serialize(_smallObject);
     
    [Benchmark, BenchmarkCategory("Serialize", "Small")]
    public string Newtonsoft_Serialize_Small() =>
        JsonConvert.SerializeObject(_smallObject);
    //  
    // [Benchmark, BenchmarkCategory("Serialize", "Small")]
    // public string Jil_Serialize_Small() =>
    //     JSON.Serialize(_smallObject);
    //  
    // [Benchmark, BenchmarkCategory("Serialize", "Small")]
    // public string Utf8Json_Serialize_Small() =>
    //     Utf8Json.JsonSerializer.ToJsonString(_smallObject);
    //  
    // [Benchmark, BenchmarkCategory("Serialize", "Small")]
    // public string SpanJson_Serialize_Small() =>
    //     SpanJson.JsonSerializer.Generic.Utf16.Serialize(_smallObject);

    #endregion

    // #region SerializeMedium
    //
    // [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Medium")]
    // public string STJ_Serialize_Medium() =>
    //      System.Text.Json.JsonSerializer.Serialize(_mediumObject);
    //  
    // [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    // public string Newtonsoft_Serialize_Medium() =>
    //      JsonConvert.SerializeObject(_mediumObject);
    //  
    // [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    // public string Jil_Serialize_Medium() =>
    //      JSON.Serialize(_mediumObject);
    //  
    //  [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    // public string Utf8Json_Serialize_Medium() =>
    //      Utf8Json.JsonSerializer.ToJsonString(_mediumObject);
    //
    // [Benchmark, BenchmarkCategory("Serialize", "Medium")]
    // public string SpanJson_Serialize_Medium() =>
    //      SpanJson.JsonSerializer.Generic.Utf16.Serialize(_mediumObject);
    //
    //  #endregion
    //  
    //  #region SerializeLarge
    //
    //  [Benchmark(Baseline = true), BenchmarkCategory("Serialize", "Large")]
    //  public string STJ_Serialize_Large() =>
    //      System.Text.Json.JsonSerializer.Serialize(_largeObject);
    //  
    //  [Benchmark, BenchmarkCategory("Serialize", "Large")]
    //  public string Newtonsoft_Serialize_Large() =>
    //      JsonConvert.SerializeObject(_largeObject);
    //  
    //  [Benchmark, BenchmarkCategory("Serialize", "Large")]
    //  public string Jil_Serialize_Large() =>
    //      JSON.Serialize(_largeObject);
    //  
    //  [Benchmark, BenchmarkCategory("Serialize", "Large")]
    //  public string Utf8Json_Serialize_Large() =>
    //      Utf8Json.JsonSerializer.ToJsonString(_largeObject);
    //  
    //  [Benchmark, BenchmarkCategory("Serialize", "Large")]
    //  public string SpanJson_Serialize_Large() =>
    //      SpanJson.JsonSerializer.Generic.Utf16.Serialize(_largeObject);
    //  
    //  #endregion
    //  
    //  #region DeserializeSmall
    //  
    //  [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Small")]
    //  public CommunityProfile STJ_Deserialize_Small() =>
    //      System.Text.Json.JsonSerializer.Deserialize<CommunityProfile>(_smallJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    //  public CommunityProfile Newtonsoft_Deserialize_Small() =>
    //      JsonConvert.DeserializeObject<CommunityProfile>(_smallJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    //  public CommunityProfile Jil_Deserialize_Small() =>
    //      JSON.Deserialize<CommunityProfile>(_smallJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    //  public CommunityProfile Utf8Json_Deserialize_Small() =>
    //      Utf8Json.JsonSerializer.Deserialize<CommunityProfile>(_smallJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Small")]
    //  public CommunityProfile SpanJson_Deserialize_Small() =>
    //      SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommunityProfile>(_smallJson);
    //  #endregion
    //  
    //  #region DeserializeMedium
    //  [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Medium")]
    //  public CommitItem STJ_Deserialize_Medium() =>
    //      System.Text.Json.JsonSerializer.Deserialize<CommitItem>(_mediumJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    //  public CommitItem Newtonsoft_Deserialize_Medium() =>
    //      JsonConvert.DeserializeObject<CommitItem>(_mediumJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    //  public CommitItem Jil_Deserialize_Medium() =>
    //      JSON.Deserialize<CommitItem>(_mediumJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    //  public CommitItem Utf8Json_Deserialize_Medium() =>
    //      Utf8Json.JsonSerializer.Deserialize<CommitItem>(_mediumJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Medium")]
    //  public CommitItem SpanJson_Deserialize_Medium() =>
    //      SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitItem>(_mediumJson);
    //  
    //  #endregion
    //  
    //  #region DeserializeLarge
    //
    //  [Benchmark(Baseline = true), BenchmarkCategory("Deserialize", "Large")]
    //  public CommitCompare STJ_Deserialize_Large() =>
    //      System.Text.Json.JsonSerializer.Deserialize<CommitCompare>(_largeJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    //  public CommitCompare Newtonsoft_Deserialize_Large() =>
    //      JsonConvert.DeserializeObject<CommitCompare>(_largeJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    //  public CommitCompare Jil_Deserialize_Large() =>
    //      JSON.Deserialize<CommitCompare>(_largeJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    //  public CommitCompare Utf8Json_Deserialize_Large() =>
    //      Utf8Json.JsonSerializer.Deserialize<CommitCompare>(_largeJson);
    //  
    //  [Benchmark, BenchmarkCategory("Deserialize", "Large")]
    //  public CommitCompare SpanJson_Deserialize_Large() =>
    //      SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitCompare>(_largeJson);
    //  
    //  #endregion
    //
    //  #region Minbenchmarks
    //  
    // [Benchmark, BenchmarkCategory("Test", "Empty")]
    // public void Empty() { }
    //
    // private int _x;
    // [Benchmark, BenchmarkCategory("Test", "Increment")]
    // public int SimpleIncrement()
    // {
    //     _x++;
    //     return _x;
    // }
    // #endregion
}