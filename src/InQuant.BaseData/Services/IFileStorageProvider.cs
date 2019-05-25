using System.IO;
using System.Threading.Tasks;

namespace InQuant.BaseData.Services
{
    public interface IFileStorageProvider
    {
        ValueTask<string> GetPublicUrl(string fileName);

        Task Save(string fileName, Stream stream);

        Task<Stream> Open(string fileName);

        Task<bool> Exists(string fileName);

        Task Delete(string fileName);
    }
}
