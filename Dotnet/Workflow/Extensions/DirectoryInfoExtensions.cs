namespace Workflow.Extensions;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo GetOrCreateDirectory(this string path)
    {
        return !Directory.Exists(path) ? Directory.CreateDirectory(path) : new DirectoryInfo(path);
    }
    
    public static DirectoryInfo GetOrCreateSubDirectory(this DirectoryInfo directoryInfo, string subDirectoryName)
    {
        var subDirectoryPath = Path.Combine(directoryInfo.FullName, subDirectoryName);
        return !Directory.Exists(subDirectoryPath) ? Directory.CreateDirectory(subDirectoryPath) : new DirectoryInfo(subDirectoryPath);
    }
    
    public static async Task CreateFileAsync(this DirectoryInfo directoryInfo, string fileName, string? content = null)
    {
        var filePath = Path.Combine(directoryInfo.FullName, fileName);
        await File.WriteAllTextAsync(filePath, content);
    }
    
    public static int GetNextId(this DirectoryInfo directoryInfo)
    {
        var directories = directoryInfo.GetDirectories();
        var maxId = 0;

        if (directories.Any())
        {
            maxId = directories.Select(x => int.Parse(x.Name)).Max();
        }

        return maxId + 1;
    }
    
    public static FileInfo? GetDescriptionFile(this IEnumerable<FileInfo> fileInfos) =>
        fileInfos.FirstOrDefault(x => x.Name == Constants.DescriptionFile);

    public static DateTime? IsDeleted(this IEnumerable<FileInfo> fileInfos) =>
        fileInfos.FirstOrDefault(x => x.Name == Constants.DeletedFile)?.CreationTime;
    
    public static async Task CreateDescriptionFileAsync(this DirectoryInfo directoryInfo, string description) =>
        await directoryInfo.CreateFileAsync(Constants.DescriptionFile, description);
    
    public static async Task CreateDeletedFileAsync(this DirectoryInfo directoryInfo) =>
        await directoryInfo.CreateFileAsync(Constants.DeletedFile);
}