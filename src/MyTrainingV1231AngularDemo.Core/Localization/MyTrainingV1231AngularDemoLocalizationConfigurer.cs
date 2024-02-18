using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace MyTrainingV1231AngularDemo.Localization
{
    public static class MyTrainingV1231AngularDemoLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    MyTrainingV1231AngularDemoConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyTrainingV1231AngularDemoLocalizationConfigurer).GetAssembly(),
                        "MyTrainingV1231AngularDemo.Localization.MyTrainingV1231AngularDemo"
                    )
                )
            );
        }
    }
}