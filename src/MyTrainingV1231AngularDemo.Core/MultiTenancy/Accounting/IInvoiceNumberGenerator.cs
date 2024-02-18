using System.Threading.Tasks;
using Abp.Dependency;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Accounting
{
    public interface IInvoiceNumberGenerator : ITransientDependency
    {
        Task<string> GetNewInvoiceNumber();
    }
}