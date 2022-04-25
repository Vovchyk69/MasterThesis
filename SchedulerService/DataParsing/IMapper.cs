namespace SchedulerService.DataParsing;

public interface IMapper<T>
{
    void Map(T el);
}