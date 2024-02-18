using System.Collections.Generic;
using Abp.Auditing;

namespace MyTrainingV1231AngularDemo.Auditing
{
    public interface IExpiredAndDeletedAuditLogBackupService
    {
        bool CanBackup();
        
        void Backup(List<AuditLog> auditLogs);
    }
}