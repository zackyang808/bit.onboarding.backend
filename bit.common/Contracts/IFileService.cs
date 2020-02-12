using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace bit.common.Contracts
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile file, FileType type);
        Task<string> GetFilePath(string id, FileType type);
    }

    public enum FileType
    {
        PostPhoto = 1,
        WorkSample = 2
    }
}