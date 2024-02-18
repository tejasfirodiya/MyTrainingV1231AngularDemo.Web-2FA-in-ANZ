using System.Collections.Generic;
using MyTrainingV1231AngularDemo.Authorization.Users.Importing.Dto;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Importing
{
    public interface IInvalidUserExporter
    {
        FileDto ExportToFile(List<ImportUserDto> userListDtos);
    }
}
