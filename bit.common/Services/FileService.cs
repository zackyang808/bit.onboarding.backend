using System;
using System.IO;
using System.Threading.Tasks;
using bit.common.Aws;
using bit.common.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace bit.common.Services
{
    public class FileService : IFileService
    {
        private readonly IHostingEnvironment _environment;
        private readonly string _imageFolder;
        private IAwsService _awsService;
        private readonly string postPhotoBucketName = "test-s3-image";
        private readonly string workSampleBucketName = "mars-listing-photo";
        public FileService(IHostingEnvironment environment,
            IAwsService awsService)
        {
            _environment = environment;
            _imageFolder = "images/";
            _awsService = awsService;
        }

        public async Task<string> GetFilePath(string id, FileType type)
        {
            var awsPresignedUrl = "";

            switch (type)
            {
                case FileType.PostPhoto:
                    awsPresignedUrl = await _awsService.GetStaticUrl(id, postPhotoBucketName);
                    return awsPresignedUrl;
                case FileType.WorkSample:
                    awsPresignedUrl = await _awsService.GetStaticUrl(id, workSampleBucketName);
                    return awsPresignedUrl;
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task<string> SaveFile(IFormFile file, FileType type)
        {
            switch (type)
            {
                case FileType.PostPhoto:
                    return await SavePostPhoto(file);
                case FileType.WorkSample:
                    return await SaveWorkSamplePhoto(file);
                default:
                    throw new NotImplementedException();
            }
        }


        #region Document Save Methods

        private async Task<string> SavePostPhoto(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var newFileName = fileName.Insert(0, $"{Guid.NewGuid()}");
            string photoFilePath = Path.Combine(_environment.ContentRootPath, _imageFolder);

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(photoFilePath, newFileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var physicalPath = Path.Combine(photoFilePath, newFileName);
                var success = await _awsService.PutFileToS3(newFileName, physicalPath, postPhotoBucketName);
                if (success)
                {
                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }
                }
                else
                {
                    throw new ApplicationException("Uploaded file failed to push to AWS S3");
                }
                return newFileName;
            }
            return newFileName;
        }

        private async Task<string> SaveWorkSamplePhoto(IFormFile file)
        {
            var fileName = Path.GetFileName(file.FileName);
            var newFileName = fileName.Insert(0, $"{Guid.NewGuid()}");
            string photoFilePath = Path.Combine(_environment.ContentRootPath, _imageFolder);

            if (file.Length > 0)
            {
                using (var fileStream = new FileStream(Path.Combine(photoFilePath, newFileName), FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
                var physicalPath = Path.Combine(photoFilePath, newFileName);
                var success = await _awsService.PutFileToS3(newFileName, physicalPath, workSampleBucketName);
                if (success)
                {
                    if (File.Exists(physicalPath))
                    {
                        File.Delete(physicalPath);
                    }
                }
                else
                {
                    throw new ApplicationException("Uploaded file failed to push to AWS S3");
                }
                return newFileName;
            }
            return newFileName;
        }

        #endregion

    }
}