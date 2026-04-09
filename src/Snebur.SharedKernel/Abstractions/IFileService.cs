using System.Text;

namespace Snebur.SharedKernel.Abstractions;

public interface IFileService
{
    bool FileExists(string? path);
    bool DirectoryExists(string? path);
    string ReadAllText(string path, Encoding? encoding = default);

    string[] GetFiles(string path,
        string searchPattern, 
        SearchOption searchOption,
        bool throwIfDirectoryDoesNotExist = true) ;

    void WriteAllText(string path,
        string contents,
        Encoding? encoding = default);

    void CreateDirectory(string path);
    //async
    Task<string> ReadAllTextAsync(
        string path, 
        Encoding? encoding = default,
        CancellationToken cancellationToken = default);

    Task WriteAllTextAsync(
        string path,
        string contents,
        Encoding? encoding = default,
        CancellationToken cancellationToken = default);
}
