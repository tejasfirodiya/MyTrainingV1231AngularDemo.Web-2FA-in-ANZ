using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.MultiTenancy.Dto;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.Tenants
{
    [AutoMapFrom(typeof(TenantListDto))]
    [AutoMapTo(typeof(TenantEditDto), typeof(CreateTenantInput))]
    public class TenantListModel : TenantListDto
    {
 
    }
}
