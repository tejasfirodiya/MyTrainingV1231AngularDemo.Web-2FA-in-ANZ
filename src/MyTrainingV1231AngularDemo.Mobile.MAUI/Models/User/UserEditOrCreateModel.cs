using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;


namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(GetUserForEditOutput))]
    public class UserEditOrCreateModel : GetUserForEditOutput
    {
        public string Photo { get; set; }

        public string FullName => User == null ? string.Empty : User.Name + " " + User.Surname;

        public DateTime CreationTime { get; set; }

        public bool IsEmailConfirmed { get; set; }

        private List<OrganizationUnitModel> _organizationUnits;
        public List<OrganizationUnitModel> OrganizationUnits
        {
            get => _organizationUnits;
            set
            {
                _organizationUnits = value?.OrderBy(o => o.Code).ToList();
                SetAsAssignedForMemberedOrganizationUnits();
            }
        }

        private void SetAsAssignedForMemberedOrganizationUnits()
        {
            if (_organizationUnits != null)
            {
                MemberedOrganizationUnits?.ForEach(memberedOrgUnitCode =>
                {
                    _organizationUnits
                        .Single(o => o.Code == memberedOrgUnitCode)
                        .IsAssigned = true;
                });
            }
        }       
    }
}
