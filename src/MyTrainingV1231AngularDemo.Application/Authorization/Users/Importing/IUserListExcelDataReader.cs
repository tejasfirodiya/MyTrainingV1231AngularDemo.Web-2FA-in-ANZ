using System.Collections.Generic;
using MyTrainingV1231AngularDemo.Authorization.Users.Importing.Dto;
using Abp.Dependency;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Importing
{
    public interface IUserListExcelDataReader: ITransientDependency
    {
        List<ImportUserDto> GetUsersFromExcel(byte[] fileBytes);
    }
}
