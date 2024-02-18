using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Notifications;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.Authorization.Users;
using MyTrainingV1231AngularDemo.Net.Sms;

namespace MyTrainingV1231AngularDemo.Notifications
{
    public class SmsRealTimeNotifier : IRealTimeNotifier, ITransientDependency
{
    public bool UseOnlyIfRequestedAsTarget => true;

    public ILogger Logger { get; set; }
    private readonly ISmsSender _smsSender;
    private readonly IUnitOfWorkManager _unitOfWorkManager;
    private readonly IRepository<User, long> _userRepository;

    public SmsRealTimeNotifier(
        ISmsSender smsSender,
        IUnitOfWorkManager unitOfWorkManager,
        IRepository<User, long> userRepository)
    {
        _smsSender = smsSender;
        _unitOfWorkManager = unitOfWorkManager;
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
                            u.PhoneNumber,
                            u.Name
                        }
                    )
                    .ToListAsync();

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
                        Logger.Info("Can not send sms to user: " + userNotification.UserId +
                                    ". User does not exists!");
                        continue;
                    }

                    if (user.PhoneNumber.IsNullOrWhiteSpace())
                    {
                        Logger.Info("User " + user.Name + " has no phone number to send SMS.");
                        continue;
                    }

                    await _smsSender.SendAsync(user.PhoneNumber,
                        userNotification.Notification.Data["Message"].ToString()
                    );
                }
            }
        }
    }
}
}