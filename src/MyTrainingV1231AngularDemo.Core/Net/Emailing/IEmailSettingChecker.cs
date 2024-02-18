using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Net.Emailing
{
    public interface IEmailSettingsChecker
    {
        bool EmailSettingsValid();

        Task<bool> EmailSettingsValidAsync();
    }
}