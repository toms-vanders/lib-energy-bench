using BenchmarkDotNet.Attributes;

namespace Serialization.Bench;

public class RaplOverheadBenchmarks
{
    private int _x;
    [Benchmark]
    public void Empty() { }

    [Benchmark]
    public int SimpleIncrement()
    {
        _x++;
        return _x;
    }
}