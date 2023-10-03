using IT.Hashing.Benchmarks;

var bench = new HashBenchmark();

bench.Length = 100;

bench.Setup();

if (bench.Force_UInt32_CRC32() != bench.IT_UInt32_CRC32()) throw new InvalidOperationException();

if (bench.IO_UInt32_CRC32() != bench.IT_UInt32_CRC32()) throw new InvalidOperationException();

if (!bench.IO_Bytes_CRC32().SequenceEqual(bench.IT_Bytes_CRC32())) throw new InvalidOperationException();

if (bench.IO_UInt32_XXH32() != bench.IT_UInt32_XXH32()) throw new InvalidOperationException();

if (bench.IO_UInt64_XXH64() != bench.IT_UInt64_XXH64()) throw new InvalidOperationException();

if (!bench.IO_Bytes_XXH32().SequenceEqual(bench.IT_Bytes_XXH32())) throw new InvalidOperationException();

if (!bench.IO_Bytes_XXH64().SequenceEqual(bench.IT_Bytes_XXH64())) throw new InvalidOperationException();

if (!bench.IT_GOST_256_Native().SequenceEqual(bench.IT_GOST_256())) throw new InvalidOperationException();

if (!bench.IT_GOST_512_Native().SequenceEqual(bench.IT_GOST_512())) throw new InvalidOperationException();

if (!bench.IT_GOST_94_Native().SequenceEqual(bench.GOST_94())) throw new InvalidOperationException();

BenchmarkDotNet.Running.BenchmarkRunner.Run(typeof(HashBenchmark));