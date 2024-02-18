using System.Collections.Generic;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;
using MyTrainingV1231AngularDemo.Auditing.Dto;
using MyTrainingV1231AngularDemo.DataExporting.Excel.MiniExcel;
using MyTrainingV1231AngularDemo.Dto;
using MyTrainingV1231AngularDemo.Storage;

namespace MyTrainingV1231AngularDemo.Auditing.Exporting
{
    public class AuditLogListExcelExporter : MiniExcelExcelExporterBase, IAuditLogListExcelExporter
    {
        private readonly ITimeZoneConverter _timeZoneConverter;
        private readonly IAbpSession _abpSession;
        
        public AuditLogListExcelExporter(
            ITimeZoneConverter timeZoneConverter,
            IAbpSession abpSession,
            ITempFileCacheManager tempFileCacheManager)
            : base(tempFileCacheManager)
        {
            _timeZoneConverter = timeZoneConverter;
            _abpSession = abpSession;
        }

        public FileDto ExportToFile(List<AuditLogListDto> auditLogList)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var auditLog in auditLogList)
            {
                items.Add(new Dictionary<string, object>()
                {
                    {L("Time"), _timeZoneConverter.Convert(auditLog.ExecutionTime, _abpSession.TenantId, _abpSession.GetUserId())},
                    {L("UserName"), auditLog.UserName},
                    {L("Service"), auditLog.ServiceName},
                    {L("Action"), auditLog.MethodName},
                    {L("Parameters"), auditLog.Parameters},
                    {L("Duration"), auditLog.ExecutionDuration},
                    {L("IpAddress"), auditLog.ClientIpAddress},
                    {L("Client"), auditLog.ClientName},
                    {L("Browser"), auditLog.BrowserInfo},
                    {L("ErrorState"), auditLog.Exception.IsNullOrEmpty() ? L("Success") : auditLog.Exception},
                });
            }

            return CreateExcelPackage("AuditLogs.xlsx", items);
        }

        public FileDto ExportToFile(List<EntityChangeListDto> entityChangeList)
        {
            var items = new List<Dictionary<string, object>>();

            foreach (var entityChange in entityChangeList)
            {
                items.Add(new Dictionary<string, object>()
                {
                    {L("Action"), entityChange.ChangeType.ToString()},
                    {L("Object"), entityChange.EntityTypeFullName},
                    {L("UserName"), entityChange.UserName},
                    {L("Time"), _timeZoneConverter.Convert(entityChange.ChangeTime, _abpSession.TenantId, _abpSession.GetUserId())},
                });
            }

            return CreateExcelPackage("DetailedLogs.xlsx", items);
        }
    }
}
