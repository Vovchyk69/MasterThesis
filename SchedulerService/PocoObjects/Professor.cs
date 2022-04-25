using System.Text.Json.Serialization;

namespace SchedulerService.PocoObjects;

public class Professor
{
    public Professor(int id, string name)
    {
        Id = id;
        Name = name;
    }
    
    [JsonPropertyName("id")]
    public int Id { get; set; }

    public string Name { get; set; }

    // [JsonIgnore]
    // public List<ClassCourse> CourseClasses { get; set; }

    // public void AddCourses(IEnumerable<ClassCourse> courseClasses)
    // {
    //     var classCourses = courseClasses as ClassCourse[] ?? courseClasses.ToArray();
    //     if (!classCourses.Any()) return;
    //     
    //     CourseClasses.AddRange(classCourses);
    // }

    public override bool Equals(object? obj) => obj is Professor professor && Id == professor.Id;

    public override int GetHashCode() => HashCode.Combine(Id);
}