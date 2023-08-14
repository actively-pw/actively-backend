using Actively.Models.Enums;

namespace Actively.Models
{
	public class Activity
	{
        public Guid Id { get; set; }
        //public User User { get; set; }
        public string Title { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Start { get; set; }
		public int TotalTime { get; set; } // milliseconds
		public double Distance { get; set; } // km
		public double AverageSpeed { get; set; } // km/h
		private Activity()
        {
            Id = Guid.NewGuid();
            Type = ActivityType.Run;
        }
        public Activity(string title,  ActivityType type, DateTime start, int totalTime, double distance, double averageSpeed)
        {
            Id = Guid.NewGuid();
            //User = user;
            Title = title;
            Type = type;
            Start = start;
            TotalTime = totalTime;
            Distance = distance;
            AverageSpeed = averageSpeed;
        }

    }

}
