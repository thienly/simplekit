using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Minio;
using Minio.Exceptions;

namespace ProductMgt.Shared.MediaApi
{
    public class FileSystemUtilities : IFileSystemUtilities
    {
        private MinioClient _minioClient;
        private FileSystemOptions _fileSystemOptions;

        public FileSystemUtilities(IOptions<FileSystemOptions> fileSystemOptions)
        {
            
            _fileSystemOptions = fileSystemOptions.Value;
            _minioClient = new MinioClient(_fileSystemOptions.Endpoint, _fileSystemOptions.AccessKey,
                _fileSystemOptions.SecretKey);
        }

        

        private async Task CreatBucket(string bucketName)
        {
            var found = await _minioClient.BucketExistsAsync(bucketName);
            if (!found)
            {
                await _minioClient.MakeBucketAsync(bucketName, _fileSystemOptions.Location);
            }
        }

        public async Task<FileSystemResponse> Upload(string bucketName,string objectName, byte[] rawData,string contentType)
        {
            try
            {
                using (Stream stream = new MemoryStream(rawData))
                {
                    
                    await CreatBucket(bucketName);
                    await _minioClient.PutObjectAsync(bucketName, objectName, data: stream, stream.Length, contentType);
                    return new FileSystemResponse()
                    {
                        Code = 200,
                        RelativePath = $"{_fileSystemOptions.Endpoint}/{bucketName}/{objectName}"
                    };
                }
            }
            catch (MinioException e)
            {
                
                return new FileSystemResponse()
                {
                    Code = 500,
                    ErrorMessage = e.Message
                };
            }
        }
    }
}