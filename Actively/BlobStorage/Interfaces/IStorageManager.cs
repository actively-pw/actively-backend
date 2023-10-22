namespace Actively.BlobStorage.Interfaces
{
	public interface IStorageManager
	{
		Task Upload(Guid activityId, BlobType type, Stream content);
		Task DeleteActivityBlobs(Guid activityId);

	}
}
