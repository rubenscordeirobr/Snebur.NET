using System.Text;
using Snebur.SharedKernel.Abstractions;
using Microsoft.Extensions.Logging;

namespace Snebur.SharedKernel.Services;

public abstract class FileServiceBase : IFileService
{
    protected ILogger Logger { get; }
    protected Encoding DefaultEncoding { get; }
    protected string RootPath { get; }

    protected FileServiceBase(
        string rootPath,
        ILogger<FileServiceBase> logger,
        Encoding? defaultEncoding = null)
    {
        Logger = logger;
        DefaultEncoding = defaultEncoding ?? Encoding.UTF8;
        RootPath = Path.GetFullPath(rootPath);
    }

    public bool FileExists(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        path = NormalizePath(path);
        return File.Exists(path);
    }

    public bool DirectoryExists(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
            return false;

        path = NormalizePath(path);
        return Directory.Exists(path);
    }

    public string ReadAllText(string path, Encoding? encoding = null)
    {
        path = NormalizePath(path);
        return ReadAllTextInternal(path, encoding);
    }

    public void WriteAllText(string path, string contents, Encoding? encoding = null)
    {
        path = NormalizePath(path);
        WriteAllTextInternal(path, contents, encoding);
    }

    public void CreateDirectory(string path)
    {
        path = NormalizePath(path);
        CreateDirectoryInternal(path);
    }

    public Task<string> ReadAllTextAsync(
        string path,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
        path = NormalizePath(path);
        return ReadAllTextAsyncInternal(path, encoding, cancellationToken);
    }

    public Task WriteAllTextAsync(
        string path,
        string contents,
        Encoding? encoding = null,
        CancellationToken cancellationToken = default)
    {
        path = NormalizePath(path);
        return WriteAllTextAsyncInternal(path, contents, encoding, cancellationToken);
    }

    public string[] GetFiles(string path,
         string searchPattern,
         SearchOption searchOption,
         bool throwIfDirectoryDoesNotExist = true)
    {
        path = NormalizePath(path);

        return GetFilesInternal(path,
            searchPattern,
            searchOption,
            throwIfDirectoryDoesNotExist);
    }

    #region Private Methods

    private string ReadAllTextInternal(string path, Encoding? encoding)
    {
        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}", fileName: path);
            }

            return File.ReadAllText(path, encoding ?? DefaultEncoding);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading file: {Path}", path);
            throw;
        }
    }

    private void WriteAllTextInternal(string path, string contents, Encoding? encoding)
    {
        try
        {
            File.WriteAllText(path, contents, encoding ?? DefaultEncoding);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing file: {Path}", path);
            throw;
        }
    }

    private void CreateDirectoryInternal(string path)
    {
        if (Directory.Exists(path))
            return;

        try
        {
            Directory.CreateDirectory(path);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error creating directory: {Path}", path);
            throw;
        }
    }

    private async Task<string> ReadAllTextAsyncInternal(
        string path,
        Encoding? encoding,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"File not found: {path}", fileName: path);
            }
            return await File.ReadAllTextAsync(path, encoding ?? DefaultEncoding, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error reading file asynchronously: {Path}", path);
            throw;
        }
    }

    private async Task WriteAllTextAsyncInternal(
        string path,
        string contents,
        Encoding? encoding,
        CancellationToken cancellationToken)
    {
        try
        {
            EnsureParentDirectoryExists(path);
            await File.WriteAllTextAsync(path, contents, encoding ?? DefaultEncoding, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error writing file asynchronously: {Path}", path);
            throw;
        }
    }

    private string[] GetFilesInternal(
        string path,
        string searchPattern,
        SearchOption searchOption,
        bool throwIfDirectoryDoesNotExist)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                if (throwIfDirectoryDoesNotExist)
                {
                    throw new DirectoryNotFoundException($"Directory not found: {path}");
                }
                return [];
            }
            return Directory.GetFiles(path, searchPattern, searchOption);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error getting files from directory: {Path}", path);
            throw;
        }
    }

    private string NormalizePath(string path)
    {
        Guard.NotNull(path);

        if (Path.IsPathRooted(path))
        {
            return path;
        }
        return Path.GetFullPath(Path.Combine(RootPath, path));
    }

    private void EnsureParentDirectoryExists(string path)
    {
        var directoryPath = Path.GetDirectoryName(path);
        if (directoryPath != null && !Directory.Exists(directoryPath))
        {
            CreateDirectoryInternal(directoryPath);
        }
    }

    #endregion Private Methods

}
