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
        
        // for sanity check. Should be around 2x energy
        // AddJob(Job.Default
        //     .WithId("Energy-2s")
        //     .WithIterationTime(TimeInterval.FromSeconds(2)));
        
        WithArtifactsPath(SerializationHelper.ResultPath());
        
        AddLogicalGroupRules(BenchmarkLogicalGroupRule.ByCategory);
        WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest));
        //AddDiagnoser(MemoryDiagnoser.Default);
        AddDiagnoser(EnergyDiagnoser.Default);
        
        AddColumn(RankColumn.Arabic);
        AddColumn(StatisticColumn.Iterations);
        AddColumn(new InvocationCountColumn());
        // Results + plots
        AddExporter(JsonExporter.Full);
        AddExporter(CsvMeasurementsExporter.Default);  // required for R plots
        AddExporter(RPlotExporter.Default);            // generates PNGs via R
    }
}