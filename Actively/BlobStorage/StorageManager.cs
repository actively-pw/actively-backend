using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Actively.BlobStorage
{
	public class StorageManager
	{
		private readonly string _connectionString;
		private const string _geojsonRoutesContainerName = "geojson-routes";

		public StorageManager(IConfiguration configuration)
		{
			_connectionString = configuration.GetSection("AzureBlob").Value!;
		}

		private BlobClient CreateBlob(Guid activityId)
		{
			var blobName = activityId + ".geojson";
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
