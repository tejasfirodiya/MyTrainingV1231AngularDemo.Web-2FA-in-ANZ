using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using MyTrainingV1231AngularDemo.MultiTenancy.Accounting.Dto;

namespace MyTrainingV1231AngularDemo.MultiTenancy.Accounting
{
    public interface IInvoiceAppService
    {
        Task<InvoiceDto> GetInvoiceInfo(EntityDto<long> input);

        Task CreateInvoice(CreateInvoiceDto input);
    }
}
