using System.Text.Json.Serialization;

namespace SchedulerService.PocoObjects;

public class Data
{
    public Data()
    {
    }

    public Data(IEnumerable<Professor?> professors, IEnumerable<Group> groups, List<Room> rooms, IEnumerable<Course> courses, List<ClassCourse> classes)
    {
        Professors = professors;
        Groups = groups;
        Rooms = rooms;
        Courses = courses;
        Classes = classes;
    }

    [JsonPropertyName("professors")]
    public IEnumerable<Professor?> Professors { get; set; }

    [JsonPropertyName("groups")]
    public IEnumerable<Group> Groups { get; set; }

    [JsonPropertyName("rooms")]
    public List<Room> Rooms { get; set; }

    

    [JsonPropertyName("courses")]
    public IEnumerable<Course> Courses { get; set; }

    

    [JsonPropertyName("classes")]
    public List<ClassCourse> Classes { get; set; }

    public Data Map()
    {
        foreach (var @class in Classes)
        {
            var professor = GetProfessorById(@class.ProfessorId);
            var course = Courses.FirstOrDefault(p => p.Id == @class.CourseId);
            @class.SetCourseAndProfessor(course!, professor!);
        }

        //AddCoursesToStudentGroups();
        MapGroups();
        
        // var grouped = Classes.GroupBy(x => x.ProfessorId);
        // foreach (var professor in Professors)
        // {
        //     var values = grouped?.FirstOrDefault(x => x.Key == professor.Id);
        //     if (!(values?.ToList().Any() ?? false)) continue;
        //     professor.AddCourses(values);
        // }

        for (var i = 0; i < Rooms.Count; i++)
            Rooms[i].Id = i;
        
        return this;
    }

    public Room GetRoomById(int id) => Rooms.FirstOrDefault(r => r.Id == id)!;

    // private void AddCoursesToStudentGroups()
    // {
    //     foreach (var group in Groups)
    //     {
    //         var result = Classes.Where(c => c.GroupIds.Contains(group.Id));
    //         group.AddClasses(result);
    //     }
    // }

    private void MapGroups()
    {
        var groups = Classes
            .Select(c => GetClassCoursesGroup(c.GroupIds))
            .ToList();

        for (var i = 0; i < Classes.Count; i++) Classes[i].AddGroups(groups[i]!);
    }

    private IEnumerable<Group?> GetClassCoursesGroup(IEnumerable<int> ids) =>
        ids.Select(id => Groups.FirstOrDefault(g => g.Id == id));

    private Professor? GetProfessorById(int id) =>
        Professors.FirstOrDefault(p => p.Id == id);
}