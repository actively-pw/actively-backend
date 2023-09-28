using Actively.Models.Enums;
using System.Globalization;

namespace Actively.Models.DTOs
{
	public class GetActivityDto
	{
		public Guid Id { get; set;}
		public string Title { get; set;}
		public Sport Sport { get; set;}
		public string Start { get; set;}
		public Stats Stats { get; set;}
		public string RouteUrl { get; set;}
		public GetActivityDto(Activity activity)
		{
			Id = activity.Id;
			Title = activity.Title;
			Sport = activity.Sport;
			Start = activity.Start.ToUniversalTime().ToString("o");
			Stats = new Stats(activity.TotalTime, activity.Distance, activity.AverageSpeed);
			RouteUrl = "https://actively.blob.core.windows.net/geojson-routes/" + Id.ToString() + ".geojson";
		}
	}
}
