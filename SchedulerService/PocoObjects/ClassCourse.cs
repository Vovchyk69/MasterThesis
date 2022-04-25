using System.Text.Json.Serialization;

namespace SchedulerService.PocoObjects;

public class ClassCourse
{
    public ClassCourse(int professorId, int courseId, int duration, bool isLaboratory, IEnumerable<int>? groupIds)
    {
        ProfessorId = professorId;
        CourseId = courseId;
        Duration = duration;
        IsLaboratory = isLaboratory;
        GroupIds = groupIds ?? Enumerable.Empty<int>();
        Groups = new();
        NumberOfSeats = 0;
    }
    
    [JsonPropertyName("professor")]
    public int ProfessorId { get; set; }
    
    [JsonIgnore]
    public Professor Professor { get; set; }

    
    [JsonPropertyName("course")]
    public int CourseId { get; set; }
    
    [JsonIgnore]
    public Course Course { get; set; }

    public int Duration { get; set; }


    [JsonPropertyName("lab")]
    public bool IsLaboratory { get; set; }
    
    [JsonPropertyName("groups")]
    public IEnumerable<int> GroupIds { get; set; }
    
    [JsonIgnore]
    public List<Group> Groups { get; set; }
    
    [JsonIgnore]
    public int NumberOfSeats { get; set; }

    public void AddGroups(IEnumerable<Group> groups)
    {
        var arrayGroups = groups as Group[] ?? groups.ToArray();
        if (!arrayGroups.Any()) return;
        
        Groups.AddRange(arrayGroups);
        NumberOfSeats += groups.Sum(x => x.Size);
    }

    public void SetCourseAndProfessor(Course course, Professor professor)
    {
        Course = course;
        Professor = professor;
    }

    public bool GroupsOverlap(ClassCourse classCourse)
    {
        return Groups.Intersect(classCourse.Groups).Any();
    }

    public bool ProfessorOverlaps(ClassCourse classCourse)
    {
        return Professor.Equals(classCourse.Professor);
    }
}