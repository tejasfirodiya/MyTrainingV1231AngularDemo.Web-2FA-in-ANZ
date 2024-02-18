using Abp.Dependency;
using Microsoft.JSInterop;
using MyTrainingV1231AngularDemo.Localization;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI
{
    public class UserDialogsService : ISingletonDependency
    {
        public static UserDialogsService Instance;
        private IJSRuntime JS;
        public UserDialogsService()
        {
            Instance = this;
        }

        public void Initialize(IJSRuntime JS)
        {
            this.JS = JS;
        }

        public async Task Block(string selector = "")
        {
            await JS.InvokeVoidAsync("BlazorUserDialogService.block", selector);
        }

        public async Task UnBlock(string selector = "")
        {
            await JS.InvokeVoidAsync("BlazorUserDialogService.unBlock", selector);
        }

        public async Task AlertSuccess(string message, string confirmButtonText = "")
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Ok");
            }

            await JS.InvokeVoidAsync("BlazorUserDialogService.alertSuccess", message, confirmButtonText);
        }

        public async Task AlertInfo(string message, string confirmButtonText = "")
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Ok");
            }

            await JS.InvokeVoidAsync("BlazorUserDialogService.alertInfo", message, confirmButtonText);
        }

        public async Task AlertError(string message, string confirmButtonText = "")
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Ok");
            }

            await JS.InvokeVoidAsync("BlazorUserDialogService.alertError", message, confirmButtonText);
        }

        public async Task AlertWarn(string message, string confirmButtonText = "")
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Ok");
            }

            await JS.InvokeVoidAsync("BlazorUserDialogService.alertWarn", message, confirmButtonText);
        }

        public async Task<bool> Confirm(string message, string title = "", string confirmButtonText = "", string cancelButtonText = "")
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Yes");
            }

            if (string.IsNullOrWhiteSpace(cancelButtonText))
            {
                cancelButtonText = L.Localize("No");
            }

            return await JS.InvokeAsync<bool>("BlazorUserDialogService.confirm", message, title, confirmButtonText, cancelButtonText);
        }

        public async Task<string> Prompt(string message, string title = "", string confirmButtonText = "", bool showCancelButton = false, string cancelButtonText = "", bool allowOutsideClick = false)
        {
            if (string.IsNullOrWhiteSpace(confirmButtonText))
            {
                confirmButtonText = L.Localize("Yes");
            }

            if (string.IsNullOrWhiteSpace(cancelButtonText))
            {
                cancelButtonText = L.Localize("No");
            }

            return await JS.InvokeAsync<string>("BlazorUserDialogService.prompt", message, title, confirmButtonText, showCancelButton, cancelButtonText, allowOutsideClick);
        }
    }
}
