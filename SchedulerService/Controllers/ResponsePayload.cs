using SchedulerService.GeneticAlgorithm;
using SchedulerService.PocoObjects;

namespace SchedulerService.Controllers;

public class ResponsePayload
{
    public IEnumerable<KeyValuePair<ClassCourse, ReservedTimeSlot>> Reservations { get; set; }

    public static explicit operator ResponsePayload(Schedule schedule)
    {
        var config = schedule.Configuration;
        schedule.Classes.ToList().ForEach(x =>
        {
            var professor = config.Professors.FirstOrDefault(p => p.Id == x.Key.ProfessorId);
            var course = config.Courses.FirstOrDefault(p => p.Id == x.Key.CourseId);
            x.Key.SetCourseAndProfessor(course!, professor!);
        });
        
        foreach (var c in schedule.Classes)
        {
            var room = config.Rooms.FirstOrDefault(r => r.Id == c.Value.Room);
            c.Value.SetRoom(room);
        }
        
        return new() {Reservations = schedule.Classes};
    }
}