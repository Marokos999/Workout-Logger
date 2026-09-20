using System.Net.Http.Json;

namespace WorkoutLogger.Mobile.Services;

public class ApiService(HttpClient client)
{
  protected async Task<T?> GetAsync<T>(string url)
  {
    try
    {
      var response = await client.GetAsync(url);
      if (!response.IsSuccessStatusCode) return default;
      return await response.Content.ReadFromJsonAsync<T>();
    }
    catch { return default; }
  }

  protected async Task<T?> PostAsync<T>(string url, object body)
  {
    try
    {
      var response = await client.PostAsJsonAsync(url, body);
      if (!response.IsSuccessStatusCode) return default;
      return await response.Content.ReadFromJsonAsync<T>();
    }
    catch { return default; }
  }

  protected async Task PatchAsync(string url)
  {
    try { await client.PatchAsync(url, null); }
    catch { }
  }

  protected async Task DeleteAsync(string url)
  {
    try { await client.DeleteAsync(url); }
    catch { }
  }
}