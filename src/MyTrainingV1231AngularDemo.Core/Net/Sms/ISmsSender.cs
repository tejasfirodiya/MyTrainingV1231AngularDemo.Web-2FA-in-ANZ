using System.Threading.Tasks;

namespace MyTrainingV1231AngularDemo.Net.Sms
{
    public interface ISmsSender
    {
        Task SendAsync(string number, string message);
    }
}