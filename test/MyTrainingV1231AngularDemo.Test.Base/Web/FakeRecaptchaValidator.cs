using System.Threading.Tasks;
using MyTrainingV1231AngularDemo.Security.Recaptcha;

namespace MyTrainingV1231AngularDemo.Test.Base.Web
{
    public class FakeRecaptchaValidator : IRecaptchaValidator
    {
        public Task ValidateAsync(string captchaResponse)
        {
            return Task.CompletedTask;
        }
    }
}
