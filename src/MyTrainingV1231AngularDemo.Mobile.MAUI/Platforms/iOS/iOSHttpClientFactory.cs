using Flurl.Http.Configuration;
using MyTrainingV1231AngularDemo.ApiClient;
using Security;
using System.Net;
using System.Net.Http;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Platforms.iOS
{
#if IOS

    public class iOSHttpClientFactory : DefaultHttpClientFactory
    {
        /// <summary>
        /// Callback function for refresh token is expired
        /// </summary>
        public Func<Task> OnSessionTimeOut { get; set; }

        /// <summary>
        /// Callback function for access token refresh
        /// </summary>
        public Func<string, string, Task> OnAccessTokenRefresh { get; set; }

        public override HttpMessageHandler CreateMessageHandler()
        {
            var httpClientHandler = new System.Net.Http.HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

#if DEBUG
            TrustLocalDeveloperCert(httpClientHandler);
#endif

            return new AuthenticationHttpHandler(httpClientHandler)
            {
                OnSessionTimeOut = OnSessionTimeOut,
                OnAccessTokenRefresh = OnAccessTokenRefresh
            };
        }

        public override HttpClient CreateHttpClient(HttpMessageHandler handler)
        {
            var iOSHandler = new NSUrlSessionHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    return true;
                },
                TrustOverrideForUrl = IsHttpsLocalhost
            };

            return new HttpClient(iOSHandler)
            {
                // Timeouts handled per request via FlurlHttpSettings.Timeout
                Timeout = Timeout.InfiniteTimeSpan
            };
        }

        public bool IsHttpsLocalhost(NSUrlSessionHandler sender, string url, SecTrust trust)
        {
#if DEBUG
            if (url.StartsWith("https://localhost") || url.StartsWith("https://192.168"))
            {
                return true;
            }
#endif

            return false;
        }

        private static void TrustLocalDeveloperCert(HttpClientHandler messageHandler)
        {
            messageHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
        }
    }
#endif
}
