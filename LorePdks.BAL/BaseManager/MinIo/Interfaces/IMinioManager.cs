namespace LorePdks.BAL.BaseManager.MinIo.Interfaces
{
    public interface IMinioManager
    {
        public void UploadData(Stream data, string bucketName, string directory, string fileName, string ext, long length);
        public string GetDataUrl(string fileName, string bucketName, string ext, int sure);

    }
}
