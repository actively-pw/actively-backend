using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Options;

namespace Actively.Services.InputFormatters
{
	/// <summary>
	/// Helper class for <c>NewtonsoftJsonPatchInputFormatter</c> generation
	/// </summary>
	public static class MyJPIF
	{
		/// <summary>
		/// Returns <c>NewtonsoftJsonPatchInputFormatter</c>
		/// </summary>
		/// <returns></returns>
		public static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
		{
			var builder = new ServiceCollection()
				.AddLogging()
				.AddMvc()
				.AddNewtonsoftJson()
				.Services.BuildServiceProvider();

			return builder
				.GetRequiredService<IOptions<MvcOptions>>()
				.Value
				.InputFormatters
				.OfType<NewtonsoftJsonPatchInputFormatter>()
				.First();
		}
	}
}
