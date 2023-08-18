using Blazored.LocalStorage;

namespace GriffinEstimator.Client;

public class BrowserStorage
{
    private readonly ILocalStorageService _storage;

    public BrowserStorage(ILocalStorageService storage)
    {
        _storage = storage;
    }

    public async Task<T> GetAsync<T>(string key)
    {
        return await _storage.GetItemAsync<T>(key);
    }

    public async Task SetAsync<T>(string key, T value)
    {
        await _storage.SetItemAsync(key, value);
    }
}
