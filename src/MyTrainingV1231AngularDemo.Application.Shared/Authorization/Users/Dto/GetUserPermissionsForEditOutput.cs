using System.Collections.Generic;
using MyTrainingV1231AngularDemo.Authorization.Permissions.Dto;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Dto
{
    public class GetUserPermissionsForEditOutput
    {
        public List<FlatPermissionDto> Permissions { get; set; }

        public List<string> GrantedPermissionNames { get; set; }
    }
}