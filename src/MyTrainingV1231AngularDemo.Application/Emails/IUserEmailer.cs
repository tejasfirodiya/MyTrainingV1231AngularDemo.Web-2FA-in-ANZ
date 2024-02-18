using Abp.Application.Services;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Emails
{
    public interface IUserEmailer : IApplicationService
    {
        Task ReplaceBodyAndSend(string emailAddress, string subject, StringBuilder emailTemplate);
        Task SendEmail();
    }
}
