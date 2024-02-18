using Abp.Application.Services.Dto;
using GraphQL.Types;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Types
{
    public class UserPagedResultGraphType : ObjectGraphType<PagedResultDto<UserDto>>
    {
        public UserPagedResultGraphType()
        {
            Name = "UserPagedResultGraphType";
            
            Field(x => x.TotalCount);
            Field(x => x.Items, type: typeof(ListGraphType<UserType>));
        }
    }
}
