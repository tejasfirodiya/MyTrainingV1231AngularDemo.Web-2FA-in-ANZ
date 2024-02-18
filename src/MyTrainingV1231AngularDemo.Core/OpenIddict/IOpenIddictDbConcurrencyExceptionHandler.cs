using System.Threading.Tasks;
using Abp.Domain.Uow;

namespace MyTrainingV1231AngularDemo.OpenIddict
{
    public interface IOpenIddictDbConcurrencyExceptionHandler
    {
        Task HandleAsync(AbpDbConcurrencyException exception);
    }
}