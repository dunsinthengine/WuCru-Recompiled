using UnityEngine;

public class DeviceAnalyser
{
	private class Benchmark
	{
		public int _benchmarkScore;

		public int _numProcessors;

		public int _systemMemory;

		public int _graphicsMemory;

		public Benchmark(int benchmarkScore, int numProcessors, int systemMemory, int graphicsMemory)
		{
			_benchmarkScore = benchmarkScore;
			_numProcessors = numProcessors;
			_systemMemory = systemMemory;
			_graphicsMemory = graphicsMemory;
		}
	}

	private static Benchmark[] IOS_BENCHMARKS = new Benchmark[2]
	{
		new Benchmark(250, 1, 500, 128),
		new Benchmark(1000, 2, 1000, 250)
	};

	private static Benchmark[] ANDROID_BENCHMARKS = new Benchmark[3]
	{
		new Benchmark(250, 2, 500, 128),
		new Benchmark(500, 4, 2000, 512),
		new Benchmark(1000, 8, 3000, 1024)
	};

	private float _overallScore;

	private float _processorScore;

	private float _ramScore;

	private float _graphicsRamScore;

	public float _pOverallScore
	{
		get
		{
			return _overallScore;
		}
	}

	public float _pProcessorScore
	{
		get
		{
			return _processorScore;
		}
	}

	public float _pRAMScore
	{
		get
		{
			return _ramScore;
		}
	}

	public float _pGraphicsRAMScore
	{
		get
		{
			return _graphicsRamScore;
		}
	}

	public void CalculateScores()
	{
		Benchmark[] aNDROID_BENCHMARKS = ANDROID_BENCHMARKS;
		if (aNDROID_BENCHMARKS.Length == 0)
		{
			return;
		}
		int num = aNDROID_BENCHMARKS.Length;
		for (int i = 0; i < num; i++)
		{
			if (i == 0 && aNDROID_BENCHMARKS[0]._numProcessors > SystemInfo.processorCount)
			{
				_processorScore = (float)aNDROID_BENCHMARKS[0]._benchmarkScore * ((float)SystemInfo.processorCount / (float)aNDROID_BENCHMARKS[0]._numProcessors);
			}
			if (i == 0 && aNDROID_BENCHMARKS[0]._systemMemory > SystemInfo.systemMemorySize)
			{
				_ramScore = (float)aNDROID_BENCHMARKS[0]._benchmarkScore * ((float)SystemInfo.systemMemorySize / (float)aNDROID_BENCHMARKS[0]._systemMemory);
			}
			if (i == 0 && aNDROID_BENCHMARKS[0]._graphicsMemory > SystemInfo.graphicsMemorySize)
			{
				_graphicsRamScore = (float)aNDROID_BENCHMARKS[0]._benchmarkScore * ((float)SystemInfo.graphicsMemorySize / (float)aNDROID_BENCHMARKS[0]._graphicsMemory);
			}
			if (i > 0 && i < num && aNDROID_BENCHMARKS[i - 1]._numProcessors < SystemInfo.processorCount && aNDROID_BENCHMARKS[i]._numProcessors >= SystemInfo.processorCount)
			{
				float num2 = (SystemInfo.processorCount - aNDROID_BENCHMARKS[i - 1]._numProcessors) / (aNDROID_BENCHMARKS[i]._numProcessors - aNDROID_BENCHMARKS[i - 1]._numProcessors);
				_processorScore = (float)aNDROID_BENCHMARKS[i - 1]._benchmarkScore + (float)(aNDROID_BENCHMARKS[i]._benchmarkScore - aNDROID_BENCHMARKS[i - 1]._benchmarkScore) * num2;
			}
			if (i > 0 && i < num && aNDROID_BENCHMARKS[i - 1]._systemMemory < SystemInfo.systemMemorySize && aNDROID_BENCHMARKS[i]._systemMemory >= SystemInfo.systemMemorySize)
			{
				float num2 = (SystemInfo.systemMemorySize - aNDROID_BENCHMARKS[i - 1]._systemMemory) / (aNDROID_BENCHMARKS[i]._systemMemory - aNDROID_BENCHMARKS[i - 1]._systemMemory);
				_ramScore = (float)aNDROID_BENCHMARKS[i - 1]._benchmarkScore + (float)(aNDROID_BENCHMARKS[i]._benchmarkScore - aNDROID_BENCHMARKS[i - 1]._benchmarkScore) * num2;
			}
			if (i > 0 && i < num && aNDROID_BENCHMARKS[i - 1]._graphicsMemory < SystemInfo.graphicsMemorySize && aNDROID_BENCHMARKS[i]._graphicsMemory >= SystemInfo.graphicsMemorySize)
			{
				float num2 = (SystemInfo.graphicsMemorySize - aNDROID_BENCHMARKS[i - 1]._graphicsMemory) / (aNDROID_BENCHMARKS[i]._graphicsMemory - aNDROID_BENCHMARKS[i - 1]._graphicsMemory);
				_graphicsRamScore = (float)aNDROID_BENCHMARKS[i - 1]._benchmarkScore + (float)(aNDROID_BENCHMARKS[i]._benchmarkScore - aNDROID_BENCHMARKS[i - 1]._benchmarkScore) * num2;
			}
			if (i == num - 1 && aNDROID_BENCHMARKS[i]._numProcessors < SystemInfo.processorCount)
			{
				float num2 = (float)SystemInfo.processorCount / (float)aNDROID_BENCHMARKS[i]._numProcessors;
				_processorScore = (float)aNDROID_BENCHMARKS[i]._benchmarkScore * num2;
			}
			if (i == num - 1 && aNDROID_BENCHMARKS[i]._systemMemory < SystemInfo.systemMemorySize)
			{
				float num2 = (float)SystemInfo.systemMemorySize / (float)aNDROID_BENCHMARKS[i]._systemMemory;
				_ramScore = (float)aNDROID_BENCHMARKS[i]._benchmarkScore * num2;
			}
			if (i == num - 1 && aNDROID_BENCHMARKS[i]._graphicsMemory < SystemInfo.graphicsMemorySize)
			{
				float num2 = (float)SystemInfo.graphicsMemorySize / (float)aNDROID_BENCHMARKS[i]._graphicsMemory;
				_graphicsRamScore = (float)aNDROID_BENCHMARKS[i]._benchmarkScore * num2;
			}
		}
		_overallScore = (_processorScore + _ramScore + _graphicsRamScore) * 0.3333f;
	}
}
