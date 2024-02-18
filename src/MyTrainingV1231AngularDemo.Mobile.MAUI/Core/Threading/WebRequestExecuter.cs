using System.Diagnostics;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Runtime.Validation;
using Abp.UI;
using Flurl.Http;
using MyTrainingV1231AngularDemo.Extensions;
using MyTrainingV1231AngularDemo.Localization;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI;
using MyTrainingV1231AngularDemo.Services.Navigation;
using Plugin.Connectivity;

namespace MyTrainingV1231AngularDemo.Core.Threading
{
    public static class WebRequestExecuter
    {
        public static async Task Execute<TResult>(
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<Exception, Task> failCallback = null,
            Action finallyCallback = null)
        {
            if (successCallback == null)
            {
                successCallback = _ => Task.CompletedTask;
            }

            if (failCallback == null)
            {
                failCallback = _ => Task.CompletedTask;
            }

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await UserDialogsService.Instance.UnBlock();

                    var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToEnableInternetAndTryAgain"), L.Localize("NoInternet"));

                    if (accepted)
                    {
                        await Execute(func, successCallback, failCallback);
                    }
                    else
                    {
                        await failCallback(new System.Exception(L.Localize("NoInternet")));
                    }
                }
                else
                {
                    await successCallback(await func());
                }
            }
            catch (System.Exception exception)
            {
                await HandleException(exception, func, successCallback, failCallback);
            }
            finally
            {
                finallyCallback?.Invoke();
            }
        }

        public static async Task Execute(
            Func<Task> func,
            Func<Task> successCallback = null,
            Func<System.Exception, Task> failCallback = null,
            Action finallyCallback = null)
        {
            if (successCallback == null)
            {
                successCallback = () => Task.CompletedTask;
            }

            if (failCallback == null)
            {
                failCallback = _ => Task.CompletedTask;
            }

            try
            {
                if (!CrossConnectivity.Current.IsConnected)
                {
                    await UserDialogsService.Instance.UnBlock();

                    var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToTryAgain"), L.Localize("NoInternet"));

                    if (accepted)
                    {
                        await Execute(func, successCallback, failCallback);
                    }
                    else
                    {
                        await failCallback(new System.Exception(L.Localize("NoInternet")));
                    }
                }
                else
                {
                    await func();
                    await successCallback();
                }
            }
            catch (System.Exception ex)
            {
                await HandleException(ex, func, successCallback, failCallback);
            }
            finally
            {
                finallyCallback?.Invoke();
            }
        }

        private static async Task HandleException<TResult>(System.Exception exception,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.UnBlock();

            switch (exception)
            {
                case UserFriendlyException userFriendlyException:
                    await HandleUserFriendlyException(userFriendlyException, failCallback);
                    break;
                case FlurlHttpTimeoutException httpTimeoutException:
                    await HandleFlurlHttpTimeoutException(httpTimeoutException, func, successCallback, failCallback);
                    break;
                case FlurlHttpException httpException:
                    await HandleFlurlHttpException(httpException, func, successCallback, failCallback);
                    break;
                case AbpValidationException abpValidationException:
                    await HandleAbpValidationException(abpValidationException, failCallback);
                    break;
                case AbpAuthorizationException abpAuthorizationException:
                    await HandleAbpAuthorizationException(abpAuthorizationException, failCallback);
                    break;
                default:
                    await HandleDefaultException(exception, func, successCallback, failCallback);
                    break;
            }
        }

        private static async Task HandleException(System.Exception exception,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.UnBlock();

            switch (exception)
            {
                case UserFriendlyException userFriendlyException:
                    await HandleUserFriendlyException(userFriendlyException, failCallback);
                    break;
                case FlurlHttpTimeoutException httpTimeoutException:
                    await HandleFlurlHttpTimeoutException(httpTimeoutException, func, successCallback, failCallback);
                    break;
                case FlurlHttpException httpException:
                    await HandleFlurlHttpException(httpException, func, successCallback, failCallback);
                    break;
                case AbpValidationException abpValidationException:
                    await HandleAbpValidationException(abpValidationException, failCallback);
                    break;
                default:
                    await HandleDefaultException(exception, func, successCallback, failCallback);
                    break;
            }
        }

        private static async Task HandleUserFriendlyException(UserFriendlyException userFriendlyException,
            Func<System.Exception, Task> failCallback)
        {
            if (string.IsNullOrEmpty(userFriendlyException.Details))
            {
                await UserDialogsService.Instance.AlertError(userFriendlyException.Message);
            }
            else
            {
                await UserDialogsService.Instance.AlertError(userFriendlyException.Message + " " + userFriendlyException.Details);
            }

            await failCallback(userFriendlyException);
        }

        private static async Task HandleFlurlHttpTimeoutException<TResult>(
            FlurlHttpTimeoutException httpTimeoutException,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {

            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToTryAgain"), L.Localize("RequestIsTimedOut"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpTimeoutException);
            }
        }

        private static async Task HandleFlurlHttpTimeoutException(FlurlHttpTimeoutException httpTimeoutException,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToTryAgain"), L.Localize("RequestIsTimedOut"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpTimeoutException);
            }
        }

        private static async Task HandleFlurlHttpException(FlurlHttpException httpException,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            if (await new AbpExceptionHandler().HandleIfAbpResponseAsync(httpException))
            {
                await failCallback(httpException);
                return;
            }

            var httpExceptionMessage = "";
            if (Debugger.IsAttached)
            {
                httpExceptionMessage += Environment.NewLine + httpException.Message;
            }

            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(httpExceptionMessage + " " + L.Localize("DoYouWantToTryAgain"), L.Localize("HTTPException"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpException);
            }
        }

        private static async Task HandleFlurlHttpException<TResult>(FlurlHttpException httpException,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            if (await new AbpExceptionHandler().HandleIfAbpResponseAsync(httpException))
            {
                await failCallback(httpException);
                return;
            }

            var httpExceptionMessage = "";
            if (Debugger.IsAttached)
            {
                httpExceptionMessage += Environment.NewLine + httpException.Message;
            }

            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(httpExceptionMessage + " " + L.Localize("DoYouWantToTryAgain"), L.Localize("HTTPException"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(httpException);
            }
        }

        private static async Task HandleAbpValidationException(AbpValidationException abpValidationException,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.AlertError(abpValidationException.GetConsolidatedMessage());

            await failCallback(abpValidationException);
        }

        private static async Task HandleAbpAuthorizationException(AbpAuthorizationException abpAuthorizationException,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.AlertError(L.Localize("CurrentUserDidNotLoginToTheApplication"));

            var navigationService = IocManager.Instance.IocContainer.Resolve<INavigationService>();
            navigationService.NavigateTo(NavigationUrlConsts.Login);

        }

        private static async Task HandleDefaultException(System.Exception exception,
            Func<Task> func,
            Func<Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToTryAgain"), L.Localize("UnhandledWebRequestException"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(exception);
            }
        }

        private static async Task HandleDefaultException<TResult>(System.Exception exception,
            Func<Task<TResult>> func,
            Func<TResult, Task> successCallback,
            Func<System.Exception, Task> failCallback)
        {
            await UserDialogsService.Instance.UnBlock();

            var accepted = await UserDialogsService.Instance.Confirm(L.Localize("DoYouWantToTryAgain"), L.Localize("UnhandledWebRequestException"));

            if (accepted)
            {
                await Execute(func, successCallback, failCallback);
            }
            else
            {
                await failCallback(exception);
            }
        }
    }
}