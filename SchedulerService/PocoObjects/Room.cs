using System.Text.Json.Serialization;

namespace SchedulerService.PocoObjects;

public class Room
{
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("lab")]
    public bool IsLaboratory { get; set; }

    [JsonPropertyName("size")]
    public int Capacity { get; set; }
    

    public Room(int id, string name, bool isLaboratory, int capacity)
    {
        Id = id;
        Name = name;
        IsLaboratory = isLaboratory;
        Capacity = capacity;
    }
}