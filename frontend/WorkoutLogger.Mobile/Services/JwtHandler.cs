namespace WorkoutLogger.Mobile.Services;

public class JwtHandler(AuthService auth) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = await auth.GetTokenAsync();
        if (token is not null)
            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            auth.Logout();
            MainThread.BeginInvokeOnMainThread(async () =>
                await Shell.Current.GoToAsync("//login"));
        }

        return response;
    }
}