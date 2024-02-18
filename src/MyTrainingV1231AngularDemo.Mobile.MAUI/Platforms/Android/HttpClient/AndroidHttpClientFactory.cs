using System.Net.Security;
using Flurl.Http.Configuration;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Platforms.Android.HttpClient
{
    public class AndroidHttpClientFactory : DefaultHttpClientFactory
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
            var handler = new AndroidAuthenticationHttpHandler()
            {
                OnSessionTimeOut = OnSessionTimeOut,
                OnAccessTokenRefresh = OnAccessTokenRefresh,
            };

#if DEBUG
            TrustLocalDeveloperCert(handler);
#endif

            return handler;
        }

        private static void TrustLocalDeveloperCert(AndroidAuthenticationHttpHandler messageHandler)
        {
            messageHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
            {
                if (cert != null && cert.Issuer.Equals("CN=localhost"))
                    return true;
                return errors == SslPolicyErrors.None;
            };
        }
    }
}
