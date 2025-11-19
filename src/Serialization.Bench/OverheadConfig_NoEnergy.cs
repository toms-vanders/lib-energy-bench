using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;
using Perfolizer.Horology;
using Serialization.Bench.Columns;
using Serialization.Bench.Helpers;

namespace Serialization.Bench;

public class OverheadConfig_NoEnergy : ManualConfig
{
    public OverheadConfig_NoEnergy()
    {
        AddLogger(ConsoleLogger.Default);
        AddJob(Job.Default.WithIterationTime(TimeInterval.Second));
        AddColumn(RankColumn.Arabic);
        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());
        
        WithArtifactsPath(SerializationHelper.ResultPath());

        // 🔹 logger
        AddLogger(ConsoleLogger.Default);

        // 🔹 exporters (GitHub MD + JSON + CSV + R plots)
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(JsonExporter.Full);
    }

}