using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.UI;
using Abp.Web.Models;
using Flurl.Http;
using JetBrains.Annotations;
using MyTrainingV1231AngularDemo.ApiClient.Models;
using MyTrainingV1231AngularDemo.Authorization.Accounts.Dto;

namespace MyTrainingV1231AngularDemo.ApiClient
{
    public class AccessTokenManager : IAccessTokenManager, ISingletonDependency
    {
        private const string LoginUrlSegment = "api/TokenAuth/Authenticate";
        private const string RefreshTokenUrlSegment = "api/TokenAuth/RefreshToken";

        private readonly AbpAuthenticateModel _authenticateModel;
        private readonly IApplicationContext _applicationContext;
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public DateTime AccessTokenRetrieveTime { get; set; }

        [CanBeNull] public AbpAuthenticateResultModel AuthenticateResult { get; set; }

        public bool IsUserLoggedIn => AuthenticateResult?.AccessToken != null;

        public bool IsRefreshTokenExpired =>
            AuthenticateResult == null || DateTime.Now >= AuthenticateResult.RefreshTokenExpireDate;

        public AccessTokenManager(
            IApplicationContext applicationContext,
            AbpAuthenticateModel authenticateModel,
            IMultiTenancyConfig multiTenancyConfig)
        {
            _applicationContext = applicationContext;
            _authenticateModel = authenticateModel;
            _multiTenancyConfig = multiTenancyConfig;
        }

        public string GetAccessToken()
        {
            if (AuthenticateResult == null)
            {
                throw new AbpAuthorizationException("You have to authenticate first!");
            }

            return AuthenticateResult.AccessToken;
        }

        public string GetEncryptedAccessToken()
        {
            if (AuthenticateResult == null)
            {
                throw new AbpAuthorizationException("You have to authenticate first!");
            }

            return AuthenticateResult.EncryptedAccessToken;
        }

        public async Task<AbpAuthenticateResultModel> LoginAsync()
        {
            EnsureUserNameAndPasswordProvided();

            using (var client = CreateApiClient())
            {
                if (_applicationContext.CurrentTenant != null)
                {
                    client.WithHeader(
                        _multiTenancyConfig.TenantIdResolveKey,
                        _applicationContext.CurrentTenant.TenantId
                    );
                }

                var response = await client
                    .Request(LoginUrlSegment)
                    .PostJsonAsync(_authenticateModel)
                    .ReceiveJson<AjaxResponse<AbpAuthenticateResultModel>>();

                if (!response.Success || response.Result == null)
                {
                    AuthenticateResult = null;
                    throw new UserFriendlyException(response.Error.Message + ": " + response.Error.Details);
                }

                AuthenticateResult = response.Result;
                AuthenticateResult.RefreshTokenExpireDate = DateTime.Now.Add(AppConsts.RefreshTokenExpiration);

                return AuthenticateResult;
            }
        }

        public async Task<(string accessToken, string encryptedAccessToken)?> RefreshTokenAsync()
        {
            if (AuthenticateResult == null || string.IsNullOrWhiteSpace(AuthenticateResult.RefreshToken))
            {
                throw new AbpAuthorizationException("No refresh token!");
            }

            using (var client = CreateApiClient())
            {
                if (_applicationContext.CurrentTenant != null)
                {
                    client.WithHeader(
                        _multiTenancyConfig.TenantIdResolveKey,
                        _applicationContext.CurrentTenant.TenantId
                    );
                }

                try
                {
                    var response = await client.Request(RefreshTokenUrlSegment)
                        .PostUrlEncodedAsync(new { refreshToken = AuthenticateResult.RefreshToken })
                        .ReceiveJson<AjaxResponse<RefreshTokenResult>>();

                    if (!response.Success)
                    {
                        AuthenticateResult = null;
                        throw new UserFriendlyException(response.Error.Message + ": " + response.Error.Details);
                    }

                    AuthenticateResult.AccessToken = response.Result.AccessToken;
                    AuthenticateResult.EncryptedAccessToken = response.Result.EncryptedAccessToken;

                    AccessTokenRetrieveTime = DateTime.Now;

                    return (response.Result.AccessToken, response.Result.EncryptedAccessToken);
                }
                catch (Exception e)
                {

                }

                return null;
            }
        }

        public void Logout()
        {
            AuthenticateResult = null;
        }

        private void EnsureUserNameAndPasswordProvided()
        {
            if (_authenticateModel.UserNameOrEmailAddress == null ||
                _authenticateModel.Password == null)
            {
                throw new Exception("Username or password fields cannot be empty!");
            }
        }

        private static IFlurlClient CreateApiClient()
        {
            IFlurlClient client = new FlurlClient(ApiUrlConfig.BaseUrl);
            client.WithHeader("Accept", new MediaTypeWithQualityHeaderValue("application/json"));
            client.WithHeader("User-Agent", MyTrainingV1231AngularDemoConsts.AbpApiClientUserAgent);
            return client;
        }
    }
}