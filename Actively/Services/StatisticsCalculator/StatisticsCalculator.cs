using Actively.Models;
using Actively.Models.DTOs;
using Actively.Models.DTOs.Statistics;
using Actively.Services.StatisticsCalculator.Interfaces;
using System.Numerics;

namespace Actively.Services.StatisticsCalculator
{
    public class StatisticsCalculator : IStatisticsCalculator
	{
		public ActivityStatistics Calculate(AddActivityDto addActivityDto)
		{
			var totalDistanceKilometers = 0.0;
			var durationMilliseconds = 0.0;
			var maxSpeed = 0.0;
			var sumOfAscent = 0.0;
			var sumOfDescent = 0.0;

			int totalPointsCount = 0;
			foreach(var slice in addActivityDto.Route)
			{
				totalPointsCount += slice.Locations.Length;
			}

			int pointsCount = totalPointsCount >= 30 ? 30 : totalPointsCount;

			foreach(var slice in addActivityDto.Route) // liczenie czasu
			{
				if (slice.Locations.Length == 0) continue;

				// time between start/resume of recording till first location update
				durationMilliseconds += (slice.Locations[0].TimeStamp - slice.Start).TotalMilliseconds;

				for (int i=0; i<slice.Locations.Length-1; i++)
				{
					durationMilliseconds += (slice.Locations[i + 1].TimeStamp - slice.Locations[i].TimeStamp).TotalMilliseconds;
				}
			}

			foreach (var slice in addActivityDto.Route) // liczenie predkosci
			{
				if (slice.Locations.Length == 0) continue;
				var locations = slice.Locations;				

				var vectorsToAverageOld = new Vector2(0.0f, 0.0f);

				for (int i = 0; i < locations.Length - 1; i++)
				{
					double distanceKm = 0;
					double speed = 0;
					double passedTimeH = 0;

					if (i < locations.Length - pointsCount) //dlugosc wektora - i > liczba punktow
					{
						var vectorsToAverage = vectorsToAverageOld;
						double averageTime = 0.0;
						for (int j = 0; j < pointsCount - 1; j++)
						{
							vectorsToAverage += LocationOperations.ProjectedDistance(locations[i+j], locations[i + j+1], DistanceUnit.Kilometers);
							//averageTime += (locations[i + j].TimeStamp - locations[i].TimeStamp).TotalHours;
						}
						//vectorsToAverage = vectorsToAverage / 5.0f;
						averageTime = (locations[i + pointsCount].TimeStamp - locations[i].TimeStamp).TotalHours;
						distanceKm = (vectorsToAverage - vectorsToAverageOld).Length(); // czy dla old (0,0,0) jest ok?
						vectorsToAverageOld = vectorsToAverage;
						var loc1 = locations[i];
						var loc2 = locations[i + pointsCount];

						passedTimeH = (loc2.TimeStamp - loc1.TimeStamp).TotalHours;
						speed = passedTimeH == 0 ? 0.0 : distanceKm / averageTime;
						//if (i % pointsCount == 0)
						//{
						//	durationMilliseconds += passedTimeH * 60 * 60 * 1000;
						//}
					}
					else
					{
						//if (i < locations.Length-2)
						//{
						//	var loc1_final = locations[i];
						//	var loc2_final = locations[i + 1];

						//	//passedTimeH = (loc2_final.TimeStamp - loc1_final.TimeStamp).TotalHours;
						//	//durationMilliseconds += passedTimeH * 60 * 60 * 1000;
						//}
						//else
						//{
						//	passedTimeH = 0.0;
						//}
						 //todo
						distanceKm = 0.0;
					}
		
					if (speed > maxSpeed)
					{
						maxSpeed = speed;
					}

					if(i%pointsCount == 0) totalDistanceKilometers += distanceKm;

					// conversion to ms

					var loc1alt = locations[i];
					var loc2alt = locations[i + 1];
					var altitudeDifference = loc2alt.Altitude - loc1alt.Altitude;
					if (altitudeDifference > 0)
					{
						sumOfAscent += altitudeDifference;
					}
					else
					{
						sumOfDescent += -altitudeDifference;
					}
				}
			}

			return new ActivityStatistics(
				distance: totalDistanceKilometers,
				duration: (long)durationMilliseconds,
				avgSpeed: CalcAvgSpeed(totalDistanceKilometers * 1000, durationMilliseconds),
				maxSpeed: maxSpeed,
				sumOfAscent: (int)sumOfAscent,
				sumOfDescent: (int)sumOfDescent
			);
		}

		public (WeeklyStatisticsDto, YearToDateStatisticsDto, AllTimeStatisticsDto) CalculateSportSummary(List<Activity> lastWeekActivities,
			List<Activity> lastYearActivities, List<Activity> allTimeActivities)
		{
			var weekStatistics = new WeeklyStatisticsDto()
			{
				Distance = CalculateDistance(lastWeekActivities),
				ActivitiesCount = lastWeekActivities.Count,
				Time = CalculateTotalTime(lastWeekActivities)
			};

			var yearStatistics = new YearToDateStatisticsDto()
			{
				Distance = CalculateDistance(lastYearActivities),
				ActivitiesCount = lastYearActivities.Count,
				Time = CalculateTotalTime(lastYearActivities),
				ElevationGain = CalculateElevationGain(lastYearActivities)
			};

			var allTimeStatistics = new AllTimeStatisticsDto()
			{
				Distance = CalculateDistance(allTimeActivities),
				ActivitiesCount = allTimeActivities.Count,
				LongestDistance = CalculateLongestDistance(allTimeActivities)
			};

			return (weekStatistics, yearStatistics, allTimeStatistics);
		}

		private long CalculateTotalTime(List<Activity> activities)
		{
			if (!activities.Any()) return 0;

			long totalTime = 0;

			foreach (var activity in activities)
			{
				totalTime += activity.TotalTime;
			}

			return totalTime;
		}

		private double CalculateDistance(List<Activity> activities)
		{
			if (!activities.Any()) return 0;

			double distance = 0;

			foreach(var activity in activities)
			{
				distance+= activity.Distance;
			}

			return distance;
		}

		private int CalculateElevationGain(List<Activity> activities)
		{
			if (!activities.Any()) return 0;

			int elevationGain = 0;

			foreach(var activity in activities)
			{
				elevationGain += activity.SumOfAscent;
			}

			return elevationGain;
		}

		private double CalculateLongestDistance(List<Activity> activities)
		{
			if (!activities.Any()) return 0;
			return activities.Max(a => a.Distance);
		}

		private double CalcAvgSpeed(double distanceMeters, double durationMilliseconds)
		{
			if (Math.Abs(durationMilliseconds) <= double.Epsilon) return 0.0;
			var distKm = distanceMeters / 1000;
			var timeH = durationMilliseconds / 60 / 60 / 1000;
			return distKm / timeH;
		}
	}

	public static class LocationOperations
	{
		private static readonly Dictionary<DistanceUnit, double> _factors = new()
		{
			{ DistanceUnit.Meters, 6373000d },
			{ DistanceUnit.Kilometers, 6373d },
		};

		public static double Distance(Location a, Location b, DistanceUnit unit)
		{
			double difLat = DegreesToRadians(b.Latitude - a.Latitude);
			double difLon = DegreesToRadians(b.Longitude - a.Longitude);
			double lat1 = DegreesToRadians(a.Latitude);
			double lat2 = DegreesToRadians(b.Latitude);

			double value = Math.Pow(Math.Sin(difLat / 2), 2) +
						   Math.Pow(Math.Sin(difLon / 2), 2) * Math.Cos(lat1) * Math.Cos(lat2);

			return RadiansToMeters(2 * Math.Atan2(Math.Sqrt(value), Math.Sqrt(1 - value)), unit);
		}

		public static Vector2 ProjectedDistance(Location a, Location b, DistanceUnit unit)
		{
			double difLat = DegreesToRadians(b.Latitude - a.Latitude);
			double difLon = DegreesToRadians(b.Longitude - a.Longitude);

			double x = RadiansToMeters(difLat, unit);
			double y = RadiansToMeters(difLon, unit);

			return new Vector2((float)x, (float)y);
		}
		private static double RadiansToMeters(double radians, DistanceUnit unit) => radians * _factors[unit];

		private static double DegreesToRadians(double degrees) => degrees % 360.0 * Math.PI / 180.0;
	}

	public enum DistanceUnit
	{
		Meters,
		Kilometers
	}
}
