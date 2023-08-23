using Azure.Storage.Blobs;
using System.Runtime.CompilerServices;

namespace Actively.BlobStorage
{
	public class StorageManager
	{
		private readonly string _connectionString;
		private const string _geojsonRoutesContainerName = "geojson-routes";

		public StorageManager(IConfiguration configuration)
		{
			_connectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("AzureBlobConnectionString");
		}

		private BlobClient CreateBlob(Guid activityId)
		{
			var blobName = activityId + ".txt"; //geojson
			var containerClient = new BlobContainerClient(_connectionString, _geojsonRoutesContainerName);
			containerClient.CreateIfNotExistsAsync();
			return containerClient.GetBlobClient(blobName);

		}

		public async Task Upload(Guid activityId, Stream content)
		{
			var blob = CreateBlob(activityId);
			content.Position = 0;
			await blob.UploadAsync(content);
		}

	}
}
