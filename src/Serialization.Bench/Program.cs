// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Serialization.Bench;

var summary = BenchmarkRunner.Run<JsonSerializationBenchmarks>();
// BenchmarkRunner.Run<RaplOverheadBenchmarks>(new OverheadConfig_NoEnergy());
// BenchmarkRunner.Run<RaplOverheadBenchmarks>(new OverheadConfig_WithEnergy());
