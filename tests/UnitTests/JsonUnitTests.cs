using Serialization.Bench.Models;

namespace UnitTests;

public class JsonUnitTests
{
    private SmallPayload _smallPayload;
    
    [Fact]
    public void SmallPayloadTest()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "small-github-user.json");
        _smallPayload = SmallPayload.CreateSampleFromFile(path);
        
        Assert.NotNull(_smallPayload);
    }
}