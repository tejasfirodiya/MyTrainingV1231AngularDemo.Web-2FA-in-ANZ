using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Security
{
    public interface IPasswordComplexitySettingStore
    {
        Task<PasswordComplexitySetting> GetSettingsAsync();
    }
}
