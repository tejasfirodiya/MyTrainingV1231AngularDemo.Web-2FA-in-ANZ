using Abp.AutoMapper;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;

namespace MyTrainingV1231AngularDemo.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(CreateOrUpdateUserInput))]
    public class UserCreateOrUpdateModel : CreateOrUpdateUserInput
    {

    }
}
