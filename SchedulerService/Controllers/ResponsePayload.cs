using SchedulerService.GeneticAlgorithm;
using SchedulerService.PocoObjects;

namespace SchedulerService.Controllers;

public class ResponsePayload
{
    public IEnumerable<KeyValuePair<ClassCourse, ReservedTimeSlot>> Reservations { get; set; }

    public static explicit operator ResponsePayload(Schedule schedule)
    {
        var config = schedule.Configuration;
        config.Classes.ForEach(x =>
        {
            var professor = config.Professors.FirstOrDefault(p => p.Id == x.ProfessorId);
            var course = config.Courses.FirstOrDefault(p => p.Id == x.CourseId);
            x.SetCourseAndProfessor(course!, professor!);
        });
        
        return new() {Reservations = schedule.Classes};
    }
}