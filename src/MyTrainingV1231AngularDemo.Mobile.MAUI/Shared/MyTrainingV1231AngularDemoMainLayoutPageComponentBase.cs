using MyTrainingV1231AngularDemo.Core.Dependency;
using MyTrainingV1231AngularDemo.Mobile.MAUI.Services.UI;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Shared
{
    public class MyTrainingV1231AngularDemoMainLayoutPageComponentBase : MyTrainingV1231AngularDemoComponentBase
    {
        protected PageHeaderService PageHeaderService { get; set; }

        protected DomManipulatorService DomManipulatorService { get; set; }

        public MyTrainingV1231AngularDemoMainLayoutPageComponentBase()
        {
            PageHeaderService = DependencyResolver.Resolve<PageHeaderService>();
            DomManipulatorService = DependencyResolver.Resolve<DomManipulatorService>();
        }

        protected async Task SetPageHeader(string title)
        {
            PageHeaderService.Title = title;
            PageHeaderService.ClearButton();
            await DomManipulatorService.ClearModalBackdrop(JS);
        }

        protected async Task SetPageHeader(string title, List<PageHeaderButton> buttons)
        {
            PageHeaderService.Title = title;
            PageHeaderService.SetButtons(buttons);
            await DomManipulatorService.ClearModalBackdrop(JS);
        }
    }
}
