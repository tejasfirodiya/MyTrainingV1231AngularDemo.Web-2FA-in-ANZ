using Abp.Dependency;
using MyTrainingV1231AngularDemo.ApiClient;
using System.Net.Http.Headers;
using System.Net;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Platforms.Android.HttpClient
{
    internal sealed class AndroidAuthenticationHttpHandler : Xamarin.Android.Net.AndroidMessageHandler
    {

#if DEBUG
        private sealed class CustomHostnameVerifier : Java.Lang.Object, Javax.Net.Ssl.IHostnameVerifier
        {
            public bool Verify(string hostname, Javax.Net.Ssl.ISSLSession session)
            {
                return
                    Javax.Net.Ssl.HttpsURLConnection.DefaultHostnameVerifier.Verify(hostname, session)
                    || hostname == "10.0.2.2" && session.PeerPrincipal?.Name == "CN=localhost";
            }
        }

        protected override Javax.Net.Ssl.IHostnameVerifier GetSSLHostnameVerifier(Javax.Net.Ssl.HttpsURLConnection connection)
            => new CustomHostnameVerifier();
#endif

        // Copy of MyTrainingV1231AngularDemo.ApiClient.AuthenticationHttpHandler for android override
        // -----------------------------

        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private const string AuthorizationScheme = "Bearer";

        public Func<Task> OnSessionTimeOut { get; set; }

        public Func<string, string, Task> OnAccessTokenRefresh { get; set; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized &&
                HasBearerAuthorizationHeader(request))
            {
                return await HandleUnauthorizedResponse(request, response, cancellationToken);
            }

            return response;
        }

        private async Task<HttpResponseMessage> HandleUnauthorizedResponse(HttpRequestMessage request, HttpResponseMessage response, CancellationToken cancellationToken)
        {
            await _semaphore.WaitAsync(cancellationToken);

            try
            {
                var tokenManager = IocManager.Instance.IocContainer.Resolve<IAccessTokenManager>();

                if (tokenManager.IsRefreshTokenExpired)
                {
                    await HandleSessionExpired(tokenManager);
                }
                else
                {
                    response = await RefreshAccessTokenAndSendRequestAgain(request, cancellationToken, tokenManager);
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return response;
        }

        private async Task<HttpResponseMessage> RefreshAccessTokenAndSendRequestAgain(HttpRequestMessage request, CancellationToken cancellationToken, IAccessTokenManager tokenManager)
        {
            await RefreshToken(tokenManager, request);
            return await base.SendAsync(request, cancellationToken);
        }

        private async Task HandleSessionExpired(IAccessTokenManager tokenManager)
        {
            tokenManager.Logout();

            if (OnSessionTimeOut != null)
            {
                await OnSessionTimeOut();
            }
        }

        private async Task RefreshToken(IAccessTokenManager tokenManager, HttpRequestMessage request)
        {
            var newTokens = await tokenManager.RefreshTokenAsync();

            if (newTokens.HasValue)
            {
                if (OnAccessTokenRefresh != null)
                {
                    await OnAccessTokenRefresh(newTokens.Value.accessToken, newTokens.Value.encryptedAccessToken);
                }

                SetRequestAccessToken(newTokens.Value.accessToken, request);
            }
            else
            {
                await HandleSessionExpired(tokenManager);
            }
        }

        private static bool HasBearerAuthorizationHeader(HttpRequestMessage request)
        {
            if (request.Headers.Authorization == null)
            {
                return false;
            }

            if (request.Headers.Authorization.Scheme != AuthorizationScheme)
            {
                return false;
            }

            return true;
        }

        private static void SetRequestAccessToken(string accessToken, HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(accessToken))
            {
                throw new ApplicationException("Cannot handle empty access token!");
            }

            request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, accessToken);
        }
    }
}
