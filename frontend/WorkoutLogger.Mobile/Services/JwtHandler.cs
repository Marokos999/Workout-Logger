namespace WorkoutLogger.Mobile.Services;

public class JwtHandler(AuthService auth) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await AttachTokenAsync(request);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            var refreshed = await auth.RefreshAsync();
            if (refreshed)
            {
                // Retry with new access token (clone request — HttpRequestMessage is not reusable)
                var retryRequest = await CloneRequestAsync(request);
                await AttachTokenAsync(retryRequest);
                response = await base.SendAsync(retryRequest, cancellationToken);
            }

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                auth.Logout();
                MainThread.BeginInvokeOnMainThread(async () =>
                    await Shell.Current.GoToAsync("//login"));
            }
        }

        return response;
    }

    private async Task AttachTokenAsync(HttpRequestMessage request)
    {
        var token = await auth.GetTokenAsync();
        if (token is not null)
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage original)
    {
        var clone = new HttpRequestMessage(original.Method, original.RequestUri);
        foreach (var header in original.Headers)
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

        if (original.Content is not null)
        {
            var bytes = await original.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(bytes);
            foreach (var header in original.Content.Headers)
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        return clone;
    }
}
