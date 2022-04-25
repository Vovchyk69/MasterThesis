namespace SchedulerService.DataParsing;

public static class Utils
{
    public static Random randomizer = new Random();

    public static int Rand() => randomizer.Next(0, 32767);

    public static double Random() => randomizer.NextDouble();

    public static int Rand(int size) => randomizer.Next(size);

    public static void Seed() => new Random(DateTime.Now.Millisecond);
}