using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Organizations.Dto;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}
