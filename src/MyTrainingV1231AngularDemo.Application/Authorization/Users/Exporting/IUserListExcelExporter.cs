using System.Collections.Generic;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}