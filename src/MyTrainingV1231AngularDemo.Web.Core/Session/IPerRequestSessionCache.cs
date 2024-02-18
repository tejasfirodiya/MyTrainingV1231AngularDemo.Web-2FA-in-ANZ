using System.Threading.Tasks;
using MyTrainingV1231AngularDemo.Sessions.Dto;

namespace MyTrainingV1231AngularDemo.Web.Session
{
    public interface IPerRequestSessionCache
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformationsAsync();
    }
}
