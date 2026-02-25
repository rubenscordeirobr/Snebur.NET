namespace Snebur.Core.Extensions;

public static class TaskExtensions
{
    public static object? GetResult(this Task task)
    {
        Guard.NotNull(task);

        var taskType = task.GetType();
        if (taskType.IsGenericType)
        {
            var resultProperty = taskType.GetProperty("Result");
            return resultProperty?.GetValue(task);
        }
        return null;
    }
}
