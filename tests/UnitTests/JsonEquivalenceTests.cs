using Jil;
using Newtonsoft.Json;
using Serialization.Bench.Helpers;
using Serialization.Bench.Models;

namespace UnitTests;

public class JsonEquivalenceTests
{
    // ========= DESERIALIZATION TESTS =========
    // "Given the same JSON text, do all deserializers produce equivalent models?"
    [Fact]
    public void Deserializers_Produce_Equivalent_Models_For_SmallPayload()
    {
        var json = SerializationHelper.LoadSample<CommunityProfile>("small-profile.json").Json;

        var stj = System.Text.Json.JsonSerializer.Deserialize<CommunityProfile>(json)!;
        var newton = JsonConvert.DeserializeObject<CommunityProfile>(json)!;
        var jil = JSON.Deserialize<CommunityProfile>(json)!;
        var utf8 = Utf8Json.JsonSerializer.Deserialize<CommunityProfile>(json)!;
        var span = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommunityProfile>(json)!;

        JsonTestHelpers.AssertStructuralEqual(stj, newton);
        JsonTestHelpers.AssertStructuralEqual(stj, jil);
        JsonTestHelpers.AssertStructuralEqual(stj, utf8);
        JsonTestHelpers.AssertStructuralEqual(stj, span);
    }

    [Fact]
    public void Deserializers_Produce_Equivalent_Models_For_MediumPayload()
    {
        var json = SerializationHelper.LoadSample<CommitItem>("medium-commit.json").Json;

        var stj = System.Text.Json.JsonSerializer.Deserialize<CommitItem>(json)!;
        var newton = JsonConvert.DeserializeObject<CommitItem>(json)!;
        var jil = JSON.Deserialize<CommitItem>(json)!;
        var utf8 = Utf8Json.JsonSerializer.Deserialize<CommitItem>(json)!;
        var span = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitItem>(json)!;

        JsonTestHelpers.AssertStructuralEqual(stj, newton);
        JsonTestHelpers.AssertStructuralEqual(stj, jil);
        JsonTestHelpers.AssertStructuralEqual(stj, utf8);
        JsonTestHelpers.AssertStructuralEqual(stj, span);
    }

    [Fact]
    public void Deserializers_Produce_Equivalent_Models_For_LargePayload()
    {
        var json = SerializationHelper.LoadSample<CommitCompare>("large-compare.json").Json;

        var stj = System.Text.Json.JsonSerializer.Deserialize<CommitCompare>(json)!;
        var newton = JsonConvert.DeserializeObject<CommitCompare>(json)!;
        var jil = JSON.Deserialize<CommitCompare>(json)!;
        var utf8 = Utf8Json.JsonSerializer.Deserialize<CommitCompare>(json)!;
        var span = SpanJson.JsonSerializer.Generic.Utf16.Deserialize<CommitCompare>(json)!;

        JsonTestHelpers.AssertStructuralEqual(stj, newton);
        JsonTestHelpers.AssertStructuralEqual(stj, jil);
        JsonTestHelpers.AssertStructuralEqual(stj, utf8);
        JsonTestHelpers.AssertStructuralEqual(stj, span);
    }

    // ========= SERIALIZATION ROUND-TRIP TESTS =========
    // "Given the same model, does each serializer preserve the data?"
    [Fact]
    public void Serializers_RoundTrip_Produce_Equivalent_Models_For_SmallPayload()
    {
        var sample = SerializationHelper.LoadSample<CommunityProfile>("small-profile.json");
        var baseline = sample.Payload; // produced via System.Text.Json in helper

        var newton = JsonTestHelpers.NormalizeFromJson<CommunityProfile>(JsonConvert.SerializeObject(baseline));
        var jil = JsonTestHelpers.NormalizeFromJson<CommunityProfile>(JSON.Serialize(baseline));
        var utf8 = JsonTestHelpers.NormalizeFromJson<CommunityProfile>(Utf8Json.JsonSerializer.ToJsonString(baseline));
        var span = JsonTestHelpers.NormalizeFromJson<CommunityProfile>(SpanJson.JsonSerializer.Generic.Utf16.Serialize(baseline));

        JsonTestHelpers.AssertStructuralEqual(baseline, newton);
        JsonTestHelpers.AssertStructuralEqual(baseline, jil);
        JsonTestHelpers.AssertStructuralEqual(baseline, utf8);
        JsonTestHelpers.AssertStructuralEqual(baseline, span);
    }

    [Fact]
    public void Serializers_RoundTrip_Produce_Equivalent_Models_For_MediumPayload()
    {
        var sample = SerializationHelper.LoadSample<CommitItem>("medium-commit.json");
        var baseline = sample.Payload;

        var newton = JsonTestHelpers.NormalizeFromJson<CommitItem>(JsonConvert.SerializeObject(baseline));
        var jil = JsonTestHelpers.NormalizeFromJson<CommitItem>(JSON.Serialize(baseline));
        var utf8 = JsonTestHelpers.NormalizeFromJson<CommitItem>(Utf8Json.JsonSerializer.ToJsonString(baseline));
        var span = JsonTestHelpers.NormalizeFromJson<CommitItem>(SpanJson.JsonSerializer.Generic.Utf16.Serialize(baseline));

        JsonTestHelpers.AssertStructuralEqual(baseline, newton);
        JsonTestHelpers.AssertStructuralEqual(baseline, jil);
        JsonTestHelpers.AssertStructuralEqual(baseline, utf8);
        JsonTestHelpers.AssertStructuralEqual(baseline, span);
    }

    [Fact]
    public void Serializers_RoundTrip_Produce_Equivalent_Models_For_LargePayload()
    {
        var sample = SerializationHelper.LoadSample<CommitCompare>("large-compare.json");
        var baseline = sample.Payload;

        var newton = JsonTestHelpers.NormalizeFromJson<CommitCompare>(JsonConvert.SerializeObject(baseline));
        var jil = JsonTestHelpers.NormalizeFromJson<CommitCompare>(JSON.Serialize(baseline));
        var utf8 = JsonTestHelpers.NormalizeFromJson<CommitCompare>(Utf8Json.JsonSerializer.ToJsonString(baseline));
        var span = JsonTestHelpers.NormalizeFromJson<CommitCompare>(SpanJson.JsonSerializer.Generic.Utf16.Serialize(baseline));

        JsonTestHelpers.AssertStructuralEqual(baseline, newton);
        JsonTestHelpers.AssertStructuralEqual(baseline, jil);
        JsonTestHelpers.AssertStructuralEqual(baseline, utf8);
        JsonTestHelpers.AssertStructuralEqual(baseline, span);
    }
}