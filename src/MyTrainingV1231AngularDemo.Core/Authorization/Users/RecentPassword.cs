using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace MyTrainingV1231AngularDemo.Authorization.Users;

[Table("AppRecentPasswords")]
public class RecentPassword : CreationAuditedEntity<Guid>, IMayHaveTenant
{
    public virtual int? TenantId { get; set; }

    [Required]
    public virtual long UserId { get; set; }
    
    [Required]
    public virtual string Password { get; set; }

    public RecentPassword()
    {
        Id = SequentialGuidGenerator.Instance.Create();
    }
}