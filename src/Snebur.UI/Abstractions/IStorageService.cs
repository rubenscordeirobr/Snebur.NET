namespace Snebur.UI.Abstractions;

public interface IStorageService
{
    Task<T?> GetItemAsync<T>(string storageKey);
    Task<string?> GetItemAsync(string storageKey);
    Task SetItemAsync(string storageKey, string textValue, bool isPersistent);
    Task SetItemAsync<T>(string storageKey, T value, bool isPersistent);
    Task RemoveItemAsync(string storageKey);
}
