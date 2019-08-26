using System.Threading.Tasks;
using Windows.Storage;

namespace Template10.Services.File
{
    public interface IFileService
    {
        Task<bool> DeleteFileAsync(string key, StorageStrategies location = StorageStrategies.Local, string path = null);
        Task<bool> FileExistsAsync(string key, StorageStrategies location = StorageStrategies.Local, string path = null);
        Task<bool> FileExistsAsync(string key, StorageFolder folder);
        Task<T> ReadFileAsync<T>(string key, StorageStrategies location = StorageStrategies.Local, string path = null);
        Task<string> ReadStringAsync(string key, StorageStrategies location = StorageStrategies.Local, string path = null);
        Task<bool> WriteFileAsync<T>(string key, T value, StorageStrategies location = StorageStrategies.Local, CreationCollisionOption option = CreationCollisionOption.ReplaceExisting, string path = null);
        Task<bool> WriteStringAsync(string key, string value, StorageStrategies location = StorageStrategies.Local, CreationCollisionOption option = CreationCollisionOption.ReplaceExisting, string path = null);
    }
}