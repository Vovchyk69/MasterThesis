using SchedulerService.DataParsing;
using SchedulerService.PocoObjects;

namespace SchedulerService.GeneticAlgorithm;

public class Schedule :IChromosome<Schedule>
{
	public float Fitness { get; private set; }

	public Data Configuration { get; private set; }

	public Dictionary<ClassCourse, ReservedTimeSlot> Classes { get; set; }
	
	public bool[] Criteria { get; private set; }

	public List<ClassCourse>[] Slots { get; private set; }

	public float Diversity { get; set; }

	public int Rank { get; set; }
	
	public Schedule(Data configuration)
	{
		Configuration = configuration;			
		Fitness = 0;

		Slots = new List<ClassCourse>[Constant.DAYS_NUM * Constant.DAY_HOURS * Configuration.Rooms.Count()];
		for(var i=0; i< Slots.Length; ++i)
			Slots[i] = new List<ClassCourse>();
		
		Classes = new Dictionary<ClassCourse, ReservedTimeSlot>();

		Criteria = new bool[Configuration.Classes.Count() * Constant.DAYS_NUM];
	}

	private Schedule Copy(Schedule c, bool setupOnly)
	{			
		if (!setupOnly) return (Schedule)c.MemberwiseClone();

		return new Schedule(c.Configuration);
	}

	public Schedule MakeNewFromPrototype()
	{
		var newChromosome = Copy(this, true);			

		var classCourses = Configuration.Classes;
		var numberOfRooms = Configuration.Rooms.Count();
		
		var day = Utils.Rand() % Constant.DAYS_NUM;
		foreach (var courseClass in classCourses)
		{
			var room = Utils.Rand() % numberOfRooms;
			var time = Utils.Rand() % (Constant.DAY_HOURS + 1 - courseClass.Duration);
			
			var reservation = new ReservedTimeSlot(numberOfRooms, day, time, room);

			for (var i = courseClass.Duration - 1; i >= 0; i--)
				newChromosome.Slots[reservation.GetHashCode() + i].Add(courseClass);

			newChromosome.Classes[courseClass] = reservation;
		}

		newChromosome.CalculateFitness();
		return newChromosome;
	}

	public Schedule Crossover(Schedule parent2, int numberOfCrossoverPoints, float crossoverProbability)
	{
		if (Utils.Rand() % 100 > crossoverProbability)
			return Copy(this, false);

		var n = Copy(this, true);

		var size = Classes.Count;
		var cp = new bool[size];

		for (int i = numberOfCrossoverPoints; i > 0; i--)
		{
			for(; ;)
			{
				var p = Utils.Rand() % size;
				if (cp[p]) continue;
				cp[p] = true;
				break;
			}
		}

		var first = Utils.Rand() % 2 == 0;
		for (int i = 0; i < size; ++i)
		{
			if (first)
			{
				var courseClass = Classes.Keys.ElementAt(i);
				var reservation = Classes[courseClass];
				n.Classes[courseClass] = reservation;
				
				for (int j = courseClass.Duration - 1; j >= 0; j--)
					n.Slots[reservation.GetHashCode() + j].Add(courseClass);
			}
			else
			{
				var courseClass = parent2.Classes.Keys.ElementAt(i);
				var reservation = parent2.Classes[courseClass];
				n.Classes[courseClass] = reservation;
				
				for (int j = courseClass.Duration - 1; j >= 0; j--)
					n.Slots[reservation.GetHashCode() + j].Add(courseClass);
			}

			if (cp[i])
				first = !first;
		}

		n.CalculateFitness();
		return n;
	}
		
	public Schedule Crossover(Schedule parent, Schedule r1, Schedule r2, Schedule r3, float etaCross, float crossoverProbability)
	{
		int size = Classes.Count;
		int jrand = Utils.Rand(size);
			
		var n = Copy(this, true);
			
		var nr = Configuration.Rooms.Count();
		for (int i = 0; i < size; ++i)
		{
			if (Utils.Rand() % 100 > crossoverProbability || i == jrand) {
				var courseClass = Classes.Keys.ElementAt(i);
				var reservation1 = r1.Classes[courseClass];
				var reservation2 = r2.Classes[courseClass];
				var reservation3 = r3.Classes[courseClass];
					
				int dur = courseClass.Duration;
				int day = (int) (reservation3.Day + etaCross * (reservation1.Day - reservation2.Day));
				if(day < 0)
					day = 0;
				else if(day >= Constant.DAYS_NUM)
					day = Constant.DAYS_NUM - 1;
					
				int room = (int) (reservation3.Room + etaCross * (reservation1.Room - reservation2.Room));
				if(room < 0)
					room = 0;
				else if(room >= nr)
					room = nr - 1;
					
				int time = (int) (reservation3.Time + etaCross * (reservation1.Time - reservation2.Time));
				if(time < 0)
					time = 0;
				else if(time >= (Constant.DAY_HOURS + 1 - dur))
					time = Constant.DAY_HOURS - dur;

				var reservation = new ReservedTimeSlot(nr, day, time, room);

				for (int j = courseClass.Duration - 1; j >= 0; --j)
					n.Slots[reservation.GetHashCode() + j].Add(courseClass);

				n.Classes[courseClass] = reservation;
			} else {
				var courseClass = parent.Classes.Keys.ElementAt(i);
				var reservation = parent.Classes[courseClass];
				n.Classes[courseClass] = reservation;
				
				for (int j = courseClass.Duration - 1; j >= 0; --j)
					n.Slots[reservation.GetHashCode() + j].Add(courseClass);
			}
		}			

		n.CalculateFitness();

		return n;
	}

	public void Mutation(int mutationSize, float mutationProbability)
	{
		if (Utils.Rand() % 100 > mutationProbability)
			return;

		int numberOfClasses = Classes.Count;
		int nr = Configuration.Rooms.Count();

		for (int i = mutationSize; i > 0; i--)
		{
			int mpos = Utils.Rand() % numberOfClasses;

			var cc1 = Classes.Keys.ElementAt(mpos);
			var reservation1 = Classes[cc1];

			int dur = cc1.Duration;
			int day = Utils.Rand() % Constant.DAYS_NUM;
			int room = Utils.Rand() % nr;
			int time = Utils.Rand() % (Constant.DAY_HOURS + 1 - dur);
			var reservation2 = new ReservedTimeSlot(nr, day, time, room);

			for (int j = dur - 1; j >= 0; j--)
			{
				var cl = Slots[reservation1.GetHashCode() + j];
				cl.RemoveAll(cc => cc == cc1);

				Slots[reservation2.GetHashCode() + j].Add(cc1);
			}

			Classes[cc1] = reservation2;
		}

		CalculateFitness();
	}

	public void CalculateFitness()
	{
		int score = 0;

		int numberOfRooms = Configuration.Rooms.Count();
		int daySize = Constant.DAY_HOURS * numberOfRooms;

		int ci = 0;
		foreach (var cc in Classes.Keys)
		{
			var reservation = Classes[cc];
			int day = reservation.Day;
			int time = reservation.Time;
			int room = reservation.Room;

			int dur = cc.Duration;

			bool ro = false;
			for (int i = dur - 1; i >= 0; i--)
			{
				if (Slots[reservation.GetHashCode() + i].Count > 1)
				{
					ro = true;
					break;
				}
			}

			if (!ro)
				score++;
			else
				score = 0;

			Criteria[ci + 0] = !ro;
				
			var r = Configuration.GetRoomById(room);
			Criteria[ci + 1] = r.Capacity >= cc.NumberOfSeats;
			if (Criteria[ci + 1])
				score++;
			else
				score /= 2;

			Criteria[ci + 2] = !cc.IsLaboratory || (cc.IsLaboratory && r.IsLaboratory);
			if (Criteria[ci + 2])
				score++;
			else
				score /= 2;

			bool po = false, go = false;
			for (int i = numberOfRooms, t = day * daySize + time; i > 0; i--, t += Constant.DAY_HOURS)
			{
				for (int j = dur - 1; j >= 0; j--)
				{
					var cl = Slots[t + j];
					foreach (var cc1 in cl)
					{
						if (cc != cc1)
						{
							if (!po && cc.ProfessorOverlaps(cc1))
								po = true;

							if (!go && cc.GroupsOverlap(cc1))
								go = true;

							if (po && go)
								goto total_overlap;
						}
					}
				}
			}

			total_overlap:

			if (!po)
				score++;
			else
				score = 0;
			Criteria[ci + 3] = !po;

			if (!go)
				score++;
			else
				score = 0;
			Criteria[ci + 4] = !go;
			ci += Constant.DAYS_NUM;
		}

		Fitness = (float)score / (Configuration.Classes.Count() * Constant.DAYS_NUM);
	}

	public override bool Equals(Object obj)
	{
		if ((obj == null) || this.GetType() != obj.GetType())
			return false;

		var other = (Schedule) obj;
		foreach (var cc in Classes.Keys)
		{
			var reservation = Classes[cc];
			var otherReservation = other.Classes[cc];
			if (!reservation.Equals(otherReservation))
				return false;
		}
		return true;
	}

	private const int Prime = 31;

	public override int GetHashCode()
	{
		var result = 1;
		foreach (var cc in Classes.Keys)
		{
			var reservation = Classes[cc];
			result = Prime * result + ((reservation == null) ? 0 : reservation.GetHashCode());
		}
		return result;
	}
}