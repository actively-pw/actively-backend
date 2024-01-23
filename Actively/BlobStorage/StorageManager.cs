using Azure.Storage.Blobs;
using MyFitBook.BlobStorage.Interfaces;

namespace MyFitBook.BlobStorage
{
	/// <summary>
	/// Azure StorageAccount helper class that allows to upload and delete files
	/// </summary>
	public class StorageManager : IStorageManager
	{
		private readonly string _connectionString;
		private readonly Dictionary<BlobType, string> _fileExtensions = new()
		{
			{BlobType.Geojson, ".geojson" },
			{BlobType.StaticMapWebLight, ".png" },
			{BlobType.StaticMapMobileLight, ".png" },
			{BlobType.StaticMapWebDark, ".png" },
			{BlobType.StaticMapMobileDark, ".png" }
		};
		private readonly Dictionary<BlobType, string> _containerNames = new()
		{
			{BlobType.Geojson, "geojson-routes" },
			{BlobType.StaticMapWebLight, "static-maps-web-light" },
			{BlobType.StaticMapMobileLight, "static-maps-mobile-light" },
			{BlobType.StaticMapWebDark, "static-maps-web-dark" },
			{BlobType.StaticMapMobileDark, "static-maps-mobile-dark" }
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="StorageManager"/> class.
		/// </summary>
		/// <param name="configuration"></param>
		public StorageManager(IConfiguration configuration)
		{
			_connectionString = configuration.GetSection("AzureBlob").Value!;
		}

		/// <summary>
		/// Uploads a new file to StorageAccount
		/// </summary>
		/// <param name="activityId"></param>
		/// <param name="type"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		public async Task Upload(Guid activityId, BlobType type, Stream content)
		{
			var blob = CreateBlob(activityId, type);
			content.Position = 0;
			await blob.UploadAsync(content);
		}

		/// <summary>
		/// Deletes all files related to activity with given <c>activityId</c>
		/// </summary>
		/// <param name="activityId"></param>
		/// <returns></returns>
		public async Task DeleteActivityBlobs(Guid activityId)
		{
			foreach (BlobType blobType in Enum.GetValues(typeof(BlobType)))
			{
				await DeleteBlob(activityId, blobType);
			}
		}

		/// <summary>
		/// Creates new blob client for a file of specified <c>type</c> related to activity given by <c>activityId</c>
		/// </summary>
		/// <param name="activityId"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		private BlobClient CreateBlob(Guid activityId, BlobType type)
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
			containerClient.CreateIfNotExistsAsync();
			return containerClient.GetBlobClient(blobName);
		}

		/// <summary>
		/// Deletes file specified by <c>type</c> and <c>activityId</c>
		/// </summary>
		/// <param name="activityId"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
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
