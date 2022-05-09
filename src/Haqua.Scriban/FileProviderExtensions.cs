using Microsoft.Extensions.FileProviders;

namespace Haqua.Scriban;

public static class FileProviderExtensions
{
    public static IEnumerable<IFileInfo> GetRecursiveFiles(
        this IFileProvider fileProvider,
        string path,
        bool includeDirectories = false)
    {
        var directoryContents = fileProvider.GetDirectoryContents(path);
        foreach (var file in directoryContents)
        {
            if (file.IsDirectory)
            {
                if (includeDirectories)
                {
                    yield return file;
                }

                var subDirectoryFiles =
                    fileProvider.GetRecursiveFiles(Path.Combine(path, file.Name), includeDirectories);
                foreach (var subDirectoryFile in subDirectoryFiles)
                {
                    yield return subDirectoryFile;
                }
            }
            else
            {
                yield return file;
            }
        }
    }
}