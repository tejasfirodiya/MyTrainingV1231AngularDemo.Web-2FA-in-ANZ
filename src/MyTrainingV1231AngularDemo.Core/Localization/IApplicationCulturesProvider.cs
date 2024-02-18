using System.Globalization;

namespace MyTrainingV1231AngularDemo.Localization
{
    public interface IApplicationCulturesProvider
    {
        CultureInfo[] GetAllCultures();
    }
}