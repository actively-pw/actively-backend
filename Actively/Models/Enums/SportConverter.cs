namespace Actively.Models.Enums
{
	/// <summary>
	/// Helper class for dealing with <c>Sport</c> enums
	/// </summary>
	public static class SportConverter
	{
		/// <summary>
		/// Converts <c>Sport</c> enum valur to string
		/// </summary>
		/// <param name="sport"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
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

		/// <summary>
		/// Converts string to <c>Sport</c> enum
		/// </summary>
		/// <param name="sport"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
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
