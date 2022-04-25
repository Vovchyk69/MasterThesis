namespace SchedulerService.PocoObjects;

public class Group
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Size { get; set; }

    public List<ClassCourse> ClassCourses { get; set; }

    public Group(int id, string name, int size)
    {
        Id = id;
        Name = name;
        Size = size;
        ClassCourses = new();
    }

    // public void AddClasses(IEnumerable<ClassCourse> classCourses) => 
    //     ClassCourses.AddRange(classCourses);
}