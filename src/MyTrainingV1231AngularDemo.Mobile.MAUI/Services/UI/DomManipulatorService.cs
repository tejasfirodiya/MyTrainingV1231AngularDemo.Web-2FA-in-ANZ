using Abp.Dependency;
using Microsoft.JSInterop;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI
{
    public class DomManipulatorService : ITransientDependency
    {
        public async Task SetAttribute(IJSRuntime JS, string jquerySelector, string attr, string value)
        {
            await JS.InvokeVoidAsync("BlazorDomManipulatorService.setAttribute", jquerySelector, attr, value);
        }

        public async Task ClearAllAttributes(IJSRuntime JS, string jquerySelector)
        {
            await JS.InvokeVoidAsync("BlazorDomManipulatorService.clearAllAttributes", jquerySelector);
        }

        public async Task ClearModalBackdrop(IJSRuntime JS)
        {
            await JS.InvokeVoidAsync("BlazorDomManipulatorService.clearModalBackdrop");
        }
    }
}
