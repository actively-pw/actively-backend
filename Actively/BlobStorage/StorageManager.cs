using Actively.BlobStorage.Interfaces;
using Azure.Storage.Blobs;

namespace Actively.BlobStorage
{
	public class StorageManager : IStorageManager
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

		public async Task Delete(Guid activityId)
		{
			var blobName = activityId + ".geojson";
			var containerClient = new BlobContainerClient(_connectionString, _geojsonRoutesContainerName);
			var blob = containerClient.GetBlobClient(blobName);
			await blob.DeleteIfExistsAsync();
		}

	}
}
