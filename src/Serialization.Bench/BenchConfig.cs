using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Exporters.Json;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using Perfolizer.Horology;
using Serialization.Bench.Columns;
using Serialization.Bench.Helpers;

namespace Serialization.Bench;

public class BenchConfig : ManualConfig
{
    public BenchConfig()
    {
        AddJob(Job.Default
            .WithId("Energy-1s")
            .WithIterationTime(TimeInterval.Second));
        
        WithArtifactsPath(SerializationHelper.ResultPath());
        WithOptions(ConfigOptions.KeepBenchmarkFiles);
        
        AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory);
        WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));

        AddDiagnoser(EnergyDiagnoser.Default);
        
        AddColumn(RankColumn.Arabic);
        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());
        AddColumn(StatisticalTestColumn.Create("3%"));

        
        AddExporter(CsvMeasurementsExporter.Default);
        AddExporter(MarkdownExporter.GitHub);
    }
}