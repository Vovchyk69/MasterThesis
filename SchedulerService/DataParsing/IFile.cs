using SchedulerService.PocoObjects;

namespace SchedulerService.DataParsing;

public interface IFile
{
    Data Parse(string filename);
}