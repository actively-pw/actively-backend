namespace Actively.BlobStorage
{
	/// <summary>
	/// Types of files that can be uploaded to Azure StorageAccount
	/// </summary>
	public enum BlobType
	{
		Geojson = 0,
		StaticMapWebLight = 1,
		StaticMapMobileLight = 2,
		StaticMapWebDark = 3,
		StaticMapMobileDark = 4
	}
}
