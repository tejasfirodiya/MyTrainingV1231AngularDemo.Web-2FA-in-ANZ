using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Security.Recaptcha
{
    public interface IRecaptchaValidator
    {
        Task ValidateAsync(string captchaResponse);
    }
}