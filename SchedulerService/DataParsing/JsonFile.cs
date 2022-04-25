using System.Text.Json;
using SchedulerService.PocoObjects;

namespace SchedulerService.DataParsing;

public class JsonFile: IFile
{
    public Data Parse(string file)
    {
        var options = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true
        };

        var deserializedData = JsonSerializer.Deserialize<Data>(file, options);
        return deserializedData!.Map();
    }
}