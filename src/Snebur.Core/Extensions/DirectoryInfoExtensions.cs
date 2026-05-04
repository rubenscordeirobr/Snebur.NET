namespace Snebur.Core.Extensions;

public static class DirectoryInfoExtensions
{
    public static DirectoryInfo GetRequiredParent(
        this DirectoryInfo directory,
        string directoryName)
    {
        Guard.NotNull(directory);
        Guard.NotNullOrWhiteSpace(directoryName);

        var parent = directory.GetParent(directoryName);
        if (parent == null)
        {
            throw new DirectoryNotFoundException($"Could not locate the '{directoryName}' directory from the {directory.FullName} directory.");
        }
        return parent;
    }

    public static DirectoryInfo? GetParent(
        this DirectoryInfo directory,
        string directoryName)
    {
        var current = directory;
        while (current != null && !current.Name.Equals(directoryName, StringComparison.OrdinalIgnoreCase))
        {
            current = current.Parent;
        }
        return current;
    }
}
