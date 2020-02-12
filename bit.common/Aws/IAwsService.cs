using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bit.common.Aws
{
    public interface IAwsService
    {
        Task<List<S3Object>> GetAllObjectFromS3(string bucketName);
        Task<S3Object> GetObjectFromName(string name, string bucketName);
        Task<string> GetPresignedUrlObject(string name, string bucketName);
        Task<string> GetStaticUrl(string name, string bucketName);
        Task<bool> PutFileToS3(string name, string filePath, string bucketName);
    }
}
