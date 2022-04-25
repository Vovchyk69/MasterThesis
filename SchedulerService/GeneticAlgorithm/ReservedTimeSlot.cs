namespace SchedulerService.GeneticAlgorithm;

public class ReservedTimeSlot
{
    public int Nr { get; }
    
    public int Day { get; }
    
    public int Time { get; }
    
    public int Room { get; }
    
    public ReservedTimeSlot(int nr, int day, int time, int room)
    {
        Nr = nr;
        Day = day;
        Time = time;
        Room = room;
    }

    public override bool Equals(Object obj)
    {
        if (obj is null || !(this.GetType() == obj.GetType()))
            return false;

        var reservation = (ReservedTimeSlot) obj;
        return GetHashCode().Equals(reservation.GetHashCode());
    }

    public override int GetHashCode()
    {
        return Day * Nr * Constant.DAY_HOURS + Room * Constant.DAY_HOURS + Time;
    }
}