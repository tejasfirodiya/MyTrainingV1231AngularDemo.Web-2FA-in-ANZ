using System.Collections.Generic;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using MyTrainingV1231AngularDemo.Authorization.Users.Dto;
using MyTrainingV1231AngularDemo.DataExporting.Excel.MiniExcel;
using MyTrainingV1231AngularDemo.Dto;
using MyTrainingV1231AngularDemo.Storage;

namespace MyTrainingV1231AngularDemo.Authorization.Users.Exporting
{
    public class UserListExcelExporter : MiniExcelExcelExporterBase, IUserListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;

        public UserListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<UserListDto> userList)
        {
            var items = new List<Dictionary<string, object>>();
            
            foreach (var user in userList)
            {
                items.Add(new Dictionary<string, object>()
                {
                    {L("Name"), user.Name},
                    {L("Surname"), user.Surname},
                    {L("UserName"), user.UserName},
                    {L("PhoneNumber"), user.PhoneNumber},
                    {L("EmailAddress"), user.EmailAddress},
                    {L("EmailConfirm"), user.IsEmailConfirmed},
                    {L("Roles"), user.Roles.Select(r => r.RoleName).JoinAsString(", ")},
                    {L("Active"), user.IsActive},
                    {
                        L("CreationTime"),
                        _timeZoneConverter.Convert(user.CreationTime, _abpSession.TenantId, _abpSession.GetUserId())
                    }
                });
            }

            return CreateExcelPackage("UserList.xlsx", items);
        }
    }
}