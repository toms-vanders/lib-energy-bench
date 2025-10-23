using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Serialization.Bench.Columns;

public class InvocationCountColumn : IColumn

{
    public string Id => nameof(InvocationCountColumn);
    public string ColumnName => "Operations";
    public bool AlwaysShow => true;
    public ColumnCategory Category => ColumnCategory.Statistics;
    public int PriorityInCategory => 0;
    public bool IsNumeric => true;
    public UnitType UnitType => UnitType.Dimensionless;
    public string Legend => "Actual invocations per measured iteration (from measurements)";
    
    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
    {
        return GetValue(summary, benchmarkCase, SummaryStyle.Default);
    }

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style)
    {
        // Pick any Workload/Actual measurement and read its Operations count
        long? ops = summary[benchmarkCase].AllMeasurements
            .Where(m => m.IterationMode == IterationMode.Workload &&
                        m.IterationStage == IterationStage.Actual)
            .Select(m => (long?)m.Operations)
            .FirstOrDefault();

        return ops.HasValue
            ? ops.Value.ToString("N0", style.CultureInfo)
            : "?";
    }

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase)
    {
        return false;
    }

    public bool IsAvailable(Summary summary)
    {
        return true;
    }
}