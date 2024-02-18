using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Net.Mail;
using Abp.Notifications;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.Authorization.Users;

namespace MyTrainingV1231AngularDemo.Notifications
{
    public class EmailRealTimeNotifier : IRealTimeNotifier, ITransientDependency
{
    public bool UseOnlyIfRequestedAsTarget => true;

    public ILogger Logger { get; set; }
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IEmailSender _emailSender;
    private readonly IRepository<User, long> _userRepository;

    public EmailRealTimeNotifier(
        IUnitOfWorkManager unitOfWorkManager,
        IEmailSender emailSender,
        IRepository<User, long> userRepository)
    {
        _unitOfWorkManager = unitOfWorkManager;
        _emailSender = emailSender;
        _userRepository = userRepository;
        Logger = NullLogger.Instance;
    }

    public async Task SendNotificationsAsync(UserNotification[] userNotifications)
    {
        var userNotificationsGroupedByTenant = userNotifications.GroupBy(un => un.TenantId);
        foreach (var userNotificationByTenant in userNotificationsGroupedByTenant)
        {
            using (_unitOfWorkManager.Current.SetTenantId(userNotificationByTenant.First().TenantId))
            {
                var allUserIds = userNotificationByTenant.ToList().Select(x => x.UserId).Distinct().ToList();
                var usersToNotify = await _userRepository.GetAll()
                    .Where(x => allUserIds.Contains(x.Id))
                    .Select(u => new
                        {
                            u.Id,
                            u.EmailAddress,
                            u.Name
                        }
                    ).ToListAsync();

                foreach (var userNotification in userNotificationByTenant)
                {
                    if (!userNotification.Notification.Data.Properties.ContainsKey("Message") ||
                        userNotification.Notification.Data["Message"] is not string)
                    {
                        Logger.Info("Message property is not found in notification data. Notification cannot be sent.");
                        continue;
                    }

                    var user = usersToNotify.FirstOrDefault(x => x.Id == userNotification.UserId);
                    if (user == null)
                    {
                        Logger.Info("Can not send sms to user: " + userNotification.UserId + ". User does not exists!");
                        continue;
                    }

                    if (user.EmailAddress.IsNullOrWhiteSpace())
                    {
                        Logger.Info("Can not send email to user: " + user.Name + ". User's email is empty!");
                        continue;
                    }

                    await _emailSender.SendAsync(new MailMessage
                    {
                        To = { user.EmailAddress },
                        Subject = "MyTrainingV1231AngularDemo Notification",
                        Body = userNotification.Notification.Data["Message"].ToString(),
                        IsBodyHtml = true
                    });
                }
            }
        }
    }
}
}