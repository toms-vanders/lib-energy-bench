// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Serialization.Bench;
using Serialization.Bench.Models;

// BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
var summary = BenchmarkRunner.Run<JsonSerializationBenchmarks>();

// var path = Path.Combine(AppContext.BaseDirectory, "small-github-user.json");
// var payload = SmallPayload.CreateSampleFromFile(path);