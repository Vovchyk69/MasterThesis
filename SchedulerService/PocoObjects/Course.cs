using System.Text.Json.Serialization;

namespace SchedulerService.PocoObjects;

public class Course
{
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Title { get; set; }

    public Course(int id, string title)
    {
        Id = id;
        Title = title;
    }
}