using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(UserListDto))]
    public class UserListModel : UserListDto
    {
        public string Photo { get; set; }

        public string FullName => Name + " " + Surname;
    }
}
