using System.Threading.Tasks;

namespace ProductMgt.Shared.MediaApi
{
    public interface IFileSystemUtilities
    {
        Task<FileSystemResponse> Upload(string bucketName,string objectName, byte[] rawData,string contentType);
    }
}