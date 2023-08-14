using Actively.Models.Enums;

namespace Actively.Models.DTOs
{
	public class GetActivityDto
	{
		public Guid Id { get; set;}
		public string Title { get; set;}
		public ActivityType Type { get; set;}
		public DateTime Start { get; set;}
		public Stats Stats { get; set;}
		public string RouteUrl { get; set;}
		public GetActivityDto(Activity activity)
		{
			Id = activity.Id;
			Title = activity.Title;
			Type = activity.Type;
			Start = activity.Start;
			Stats.TotalTime = activity.TotalTime;
			Stats.Distance = activity.Distance;
			Stats.AverageSpeed = activity.AverageSpeed;
			RouteUrl = "tu powinno byc route url";
		}
	}
}
