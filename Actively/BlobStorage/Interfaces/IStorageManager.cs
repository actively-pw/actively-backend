namespace Actively.BlobStorage.Interfaces
{
	public interface IStorageManager
	{
		Task Upload(Guid activityId, Stream content);
		Task Delete(Guid activityId);

	}
}
