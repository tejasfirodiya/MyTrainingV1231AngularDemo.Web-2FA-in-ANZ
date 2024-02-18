using System.Net;
using Abp.Web.Models;
using Castle.Core.Internal;
using Flurl.Http;
using MyTrainingV1231AngularDemo.ApiClient;
using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Extensions;
using MyTrainingV1231AngularDemo.Localization;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI;

namespace MyTrainingV1231AngularDemo.Core.Threading
{
    public class AbpExceptionHandler
    {
        public async Task<bool> HandleIfAbpResponseAsync(FlurlHttpException httpException)
        {
            AjaxResponse ajaxResponse = await httpException.GetResponseJsonAsync<AjaxResponse>();
            if (ajaxResponse == null)
            {
                return false;
            }

            if (!ajaxResponse.__abp)
            {
                return false;
            }

            if (ajaxResponse.Error == null)
            {
                return false;
            }

            if (IsUnauthroizedResponseForSessionTimoutCase(httpException, ajaxResponse))
            {
                return true;
            }

            await UserDialogsService.Instance.UnBlock();

            if (string.IsNullOrEmpty(ajaxResponse.Error.Details))
            {
                await UserDialogsService.Instance.AlertError(ajaxResponse.Error.GetConsolidatedMessage());
            }
            else
            {
                await UserDialogsService.Instance.AlertError(ajaxResponse.Error.Details, ajaxResponse.Error.GetConsolidatedMessage());
            }

            return true;
        }

        /// <summary>
        /// AuthenticationHttpHandler handles unauthorized responses and reauthenticates if there's a valid refresh token.
        /// When the refresh token expires, the application logsout and forces user to re-enter credentials
        /// That's why the last unauthorized exception can be suspended.
        /// </summary>
        private static bool IsUnauthroizedResponseForSessionTimoutCase(FlurlHttpException httpException, AjaxResponse ajaxResponse)
        {
            if (httpException.Call.HttpResponseMessage.StatusCode != HttpStatusCode.Unauthorized)
            {
                return false;
            }

            var accessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();

            var errorMsg = L.Localize("CurrentUserDidNotLoginToTheApplication", "Abp");

            if (accessTokenManager.IsUserLoggedIn)
            {
                return false;
            }

            if (!ajaxResponse.Error.Message.EqualsText(errorMsg))
            {
                return false;
            }

            return true;
        }
    }
}