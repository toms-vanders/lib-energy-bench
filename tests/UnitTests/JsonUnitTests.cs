using Serialization.Bench.Helpers;
using Serialization.Bench.Models;

namespace UnitTests;

public class JsonUnitTests
{
    private SmallPayload _smallPayload;
    
    [Fact]
    public void SmallPayloadTest()
    {
        _smallPayload = SerializationHelper.CreateSampleFromFile<SmallPayload>("small-github-user.json");
        
        Assert.NotNull(_smallPayload);
    }
}