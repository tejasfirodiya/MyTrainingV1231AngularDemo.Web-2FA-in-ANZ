using System.Globalization;
using Abp.Localization;
using Abp.Zero;
using MyTrainingV1231AngularDemo.Test.Base;
using Shouldly;
using Xunit;

namespace MyTrainingV1231AngularDemo.Tests.Localization
{
    // ReSharper disable once InconsistentNaming
    public class Localization_Tests : AppTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-AU")]
        public void Simple_Localization_Test(string cultureName)
        {
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            Resolve<ILanguageManager>().CurrentLanguage.Name.ShouldBe("en");

            var localizationManager = Resolve<ILocalizationManager>();
            var allSources = localizationManager.GetAllSources();

            localizationManager.GetString(AbpZeroConsts.LocalizationSourceName, "Identity.UserNotInRole")
                .ShouldBe("User is not in role '{0}'.");
        }
    }
}
