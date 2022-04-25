using SchedulerService.DataParsing;

namespace SchedulerService.GeneticAlgorithm;

// Genetic algorithm
public class GeneticAlgorithm<T> where T : IChromosome<T>
{
	private const int MinNumberOfChromosomes = 2;
	
	private readonly T[] _chromosomes;
	
	private readonly int _numberOfCrossoverPoints;

	private readonly int _mutationSize;
	
	private readonly float _mutationProbability;	
	
	private readonly int[] _bestChromosomes;

	private bool[] _bestFlags;

	private int _currentBestSize;

	private int _replaceByGeneration;		

	private T _prototype;
	
	private float _crossoverProbability;
	
	// Initializes genetic algorithm
	private GeneticAlgorithm(T prototype, int numberOfChromosomes, int replaceByGeneration, int trackBest)
	{
		_replaceByGeneration = replaceByGeneration;
		_currentBestSize = 0;
		_prototype = prototype;

		var filteredNumber = numberOfChromosomes < MinNumberOfChromosomes 
			? MinNumberOfChromosomes 
			: numberOfChromosomes;
		
		_chromosomes = new T[filteredNumber];
		_bestFlags = new bool[filteredNumber];

		_bestChromosomes = new int[trackBest < 1 ? 1 : trackBest];

		ReplaceByGeneration = replaceByGeneration;
	}

	public GeneticAlgorithm(T prototype, int numberOfCrossoverPoints = 2, int mutationSize = 2, float crossoverProbability = 80, float mutationProbability = 3) : this(prototype, 100, 8, 5)
	{			
		_mutationSize = mutationSize;
		_numberOfCrossoverPoints = numberOfCrossoverPoints;
		_crossoverProbability = crossoverProbability;
		_mutationProbability = mutationProbability;
	}
		
	private int ReplaceByGeneration
	{
		set => _replaceByGeneration = value > _chromosomes.Length - _bestChromosomes.Length
					? _chromosomes.Length - _bestChromosomes.Length
					: value;
	}

	public T Result => _chromosomes[_bestChromosomes[0]];

	private void AddToBest(int chromosomeIndex)
	{
		if ((_currentBestSize == _bestChromosomes.Length &&
		     _chromosomes[_bestChromosomes[_currentBestSize - 1]].Fitness >= _chromosomes[chromosomeIndex].Fitness) || _bestFlags[chromosomeIndex])
			return;

		var i = _currentBestSize;
		for (; i > 0; i--)
		{
			if (i < _bestChromosomes.Length)
			{
				if (_chromosomes[_bestChromosomes[i - 1]].Fitness > _chromosomes[chromosomeIndex].Fitness)
					break;

				_bestChromosomes[i] = _bestChromosomes[i - 1];
			}
			else
				_bestFlags[_bestChromosomes[i - 1]] = false;
		}

		_bestChromosomes[i] = chromosomeIndex;
		_bestFlags[chromosomeIndex] = true;

		if (_currentBestSize < _bestChromosomes.Length)
			_currentBestSize++;
	}		

	private bool IsInBest(int chromosomeIndex)
	{
		return _bestFlags[chromosomeIndex];
	}

	private void ClearBest()
	{
		_bestFlags = new bool[_bestFlags.Length];
		_currentBestSize = 0;
	}

	private void Initialize(T[] population)
	{
		for (int i = 0; i < population.Length; ++i)
			population[i] = _prototype.MakeNewFromPrototype();
	}

	private T[] Selection(T[] population)
	{
		var p1 = population[Utils.Rand() % population.Length];
		var p2 = population[Utils.Rand() % population.Length];
		return new T[] { p1, p2 };
	}

	private T[] Replacement(T[] population)
	{
		var offspring = new T[_replaceByGeneration];
		for (var j = 0; j < _replaceByGeneration; j++)
		{
			var parent = Selection(population);

			offspring[j] = parent[0].Crossover(parent[1], _numberOfCrossoverPoints, _crossoverProbability);
			offspring[j].Mutation(_mutationSize, _mutationProbability);

			int ci;
			do ci = Utils.Rand() % population.Length;
			while (IsInBest(ci));

			population[ci] = offspring[j];
			AddToBest(ci);
		}
		
		return offspring;
	}

	public void Run(int maxRepeat = 9999, double minFitness = 0.999)
	{
		if (_prototype == null) return;

		ClearBest();
		Initialize(_chromosomes);

		var currentGeneration = 0;
		var repeat = 0;
		var lastBestFit = 0.0;

		while (Result.Fitness <= minFitness)
		{
			Console.Write($"\rFitness: {Result.Fitness:F6}\t Generation: {currentGeneration++}");				
			
			var difference = Math.Abs(Result.Fitness - lastBestFit);
			repeat = difference <= 0.0000001 ? ++repeat : 0;
			
			if (repeat > (maxRepeat / 100))
			{
				ReplaceByGeneration = _replaceByGeneration * 3;
				++_crossoverProbability;
			}				

			Replacement(_chromosomes);

			Utils.Seed();
			lastBestFit = Result.Fitness;
		}
	}

	public override string ToString() => "Genetic Algorithm";
}