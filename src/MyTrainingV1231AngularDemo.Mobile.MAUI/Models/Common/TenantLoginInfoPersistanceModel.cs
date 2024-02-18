using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Sessions.Dto;

namespace MyTrainingV1231AngularDemo.Models.Common
{
    [AutoMapFrom(typeof(TenantLoginInfoDto)),
     AutoMapTo(typeof(TenantLoginInfoDto))]
    public class TenantLoginInfoPersistanceModel : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}