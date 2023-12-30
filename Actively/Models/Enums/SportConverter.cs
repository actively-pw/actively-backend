namespace Actively.Models.Enums
{
	public static class SportConverter
	{
		public static string SportToString(Sport sport)
		{
			switch(sport)
			{
				case Sport.Run: return "Run";
				case Sport.BicycleRide: return "Bicycle ride";
				case Sport.NordicWalking: return "Nordic walking";
				default: throw new ArgumentException($"Provided sport {sport} is invalid");
			}
		}

		public static Sport StringToSport(string sport)
		{
			sport = new string(sport.ToCharArray()
				.Where(c => !char.IsWhiteSpace(c))
				.ToArray()).ToLower();

			switch (sport)
			{
				case "run": return Sport.Run;
				case "bicycleride": return Sport.BicycleRide;
				case "nordicwalking": return Sport.NordicWalking;
				default: throw new ArgumentException($"Provided sport {sport} is invalid");
			}
		}
	}
}
