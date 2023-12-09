using Actively.BlobStorage.Interfaces;
using Azure.Storage.Blobs;

namespace Actively.BlobStorage
{
	public class StorageManager : IStorageManager
	{
		private readonly string _connectionString;
		private readonly Dictionary<BlobType, string> _fileExtensions = new()
		{
			{BlobType.Geojson, ".geojson" },
			{BlobType.StaticMapWebLight, ".png" },
			{BlobType.StaticMapMobileLight, ".png" }
		};
		private readonly Dictionary<BlobType, string> _containerNames = new()
		{
			{BlobType.Geojson, "geojson-routes" },
			{BlobType.StaticMapWebLight, "static-maps-web-light" },
			{BlobType.StaticMapMobileLight, "static-maps-mobile-light" }
		};

		public StorageManager(IConfiguration configuration)
		{
			_connectionString = configuration.GetSection("AzureBlob").Value!;
		}

		public async Task Upload(Guid activityId, BlobType type, Stream content)
		{
			var blob = CreateBlob(activityId, type);
			content.Position = 0;
			await blob.UploadAsync(content);
		}

		//deletes all blobs related to activity with given activityId
		public async Task DeleteActivityBlobs(Guid activityId)
		{
			await DeleteBlob(activityId, BlobType.Geojson);
			await DeleteBlob(activityId, BlobType.StaticMapWebLight);
			await DeleteBlob(activityId, BlobType.StaticMapMobileLight);
		}

		private BlobClient CreateBlob(Guid activityId, BlobType type)
		{
			if(!_fileExtensions.TryGetValue(type, out var fileExtension))
			{
				throw new ArgumentException("Provided blob type is invalid");
			}

			if(!_containerNames.TryGetValue(type, out var containerName))
			{
				throw new ArgumentException("Provided blob type is invalid");
			}

			var blobName = activityId + fileExtension;
			var containerClient = new BlobContainerClient(_connectionString, containerName);
			containerClient.CreateIfNotExistsAsync();
			return containerClient.GetBlobClient(blobName);

		}

		private async Task DeleteBlob(Guid activityId, BlobType type)
		{
			if (!_fileExtensions.TryGetValue(type, out var fileExtension))
			{
				throw new ArgumentException("Provided blob type is invalid");
			}

			if (!_containerNames.TryGetValue(type, out var containerName))
			{
				throw new ArgumentException("Provided blob type is invalid");
			}

			var blobName = activityId + fileExtension;
			var containerClient = new BlobContainerClient(_connectionString, containerName);
			var blob = containerClient.GetBlobClient(blobName);
			await blob.DeleteIfExistsAsync();
		}

	}
}
