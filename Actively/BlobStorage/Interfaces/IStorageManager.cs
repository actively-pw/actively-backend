using MyFitBook.BlobStorage;

namespace MyFitBook.BlobStorage.Interfaces
{
	/// <summary>
	/// Interface for Azure StorageAccount helper classes
	/// that allow to upload files related to an activity (given by <c>activityId</c>)
	/// and delete blobs related to an activity
	/// </summary>
	public interface IStorageManager
	{
		Task Upload(Guid activityId, BlobType type, Stream content);
		Task DeleteActivityBlobs(Guid activityId);

	}
}
