using Actively.Models.Enums;

namespace Actively.Models
{
	public class Activity
	{
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public ActivityType Type { get; set; }
        public DateTime Start { get; set; }
        private Activity()
        {
            Id = Guid.NewGuid();
            UserId = Guid.Empty;
            Type = ActivityType.Run;
        }
        public Activity(Guid userId, string title,  ActivityType type, DateTime start)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Title = title;
            Type = type;
            Start = start;
        }

    }
}
