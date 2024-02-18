using System;
using System.Threading.Tasks;
using Abp;
using Abp.Application.Services.Dto;
using Abp.Notifications;
using Abp.Runtime.Session;
using Abp.UI;
using MyTrainingV1231AngularDemo.Notifications;
using MyTrainingV1231AngularDemo.Notifications.Dto;
using Shouldly;
using Xunit;

namespace MyTrainingV1231AngularDemo.Tests.Notifications
{
    // ReSharper disable once InconsistentNaming
    public class NotificationAppService_Tests : AppTestBase
    {
        private readonly INotificationAppService _notificationAppService;
        private readonly INotificationStore _notificationStore;
        private readonly IGuidGenerator _guidGenerator;
        private readonly INotificationConfiguration _configuration;

        public NotificationAppService_Tests()
        {
            _notificationAppService = Resolve<INotificationAppService>();
            _notificationStore = Resolve<INotificationStore>();
            _guidGenerator = Resolve<IGuidGenerator>();
            _configuration = Resolve<INotificationConfiguration>();

            if (_configuration.Notifiers.Contains<SmsRealTimeNotifier>())
            {
                _configuration.Notifiers.Remove<SmsRealTimeNotifier>();
            }

            if (_configuration.Notifiers.Contains<EmailRealTimeNotifier>())
            {
                _configuration.Notifiers.Remove<EmailRealTimeNotifier>();
            }
        }

        [Fact]
        public async Task Test_ChangeNotificationSettings()
        {
            var settings = await _notificationAppService.GetNotificationSettings();
            settings.ReceiveNotifications.ShouldBe(true);
            settings.Notifications.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task Shouldnt_Mark_Already_Read_Notification_AsRead()
        {
            // Asset
            LoginAsDefaultTenantAdmin();

            var notificationId = await CreateUserNotificationAsync(UserNotificationState.Read);

            // Act
            var result = await _notificationAppService.SetNotificationAsRead(
                new EntityDto<Guid>
                {
                    Id = notificationId
                }
            );

            // Assert
            result.Success.ShouldBe(false);
        }

        [Fact]
        public async Task Should_Mark_Not_Read_Notification_AsRead()
        {
            // Asset
            LoginAsDefaultTenantAdmin();

            var notificationId = await CreateUserNotificationAsync(UserNotificationState.Unread);

            // Act
            var result = await _notificationAppService.SetNotificationAsRead(
                new EntityDto<Guid>
                {
                    Id = notificationId
                }
            );

            // Assert
            result.Success.ShouldBe(true);
        }

        private async Task<Guid> CreateUserNotificationAsync(UserNotificationState state)
        {
            var userNotificationId = _guidGenerator.Create();
            var tenantNotificationId = _guidGenerator.Create();

            await UsingDbContextAsync(async context =>
            {
                await context.TenantNotifications.AddAsync(new TenantNotificationInfo
                {
                    Id = tenantNotificationId,
                    TenantId = AbpSession.TenantId,
                    NotificationName = AppNotificationNames.SimpleMessage
                });

                await context.UserNotifications.AddAsync(new UserNotificationInfo
                {
                    Id = userNotificationId,
                    State = state,
                    TenantId = AbpSession.TenantId,
                    UserId = AbpSession.GetUserId(),
                    TenantNotificationId = tenantNotificationId
                });
            });

            return userNotificationId;
        }

        [Fact]
        public async Task Should_CreateMassNotification_Throw_Exception_If_There_Is_No_User()
        {
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _notificationAppService.CreateMassNotification(new CreateMassNotificationInput()
                {
                    TargetNotifiers = new[] { "MyTrainingV1231AngularDemo.Notifications.SmsRealTimeNotifier" },
                    Message = "This is a test message",
                    Severity = NotificationSeverity.Info
                });
            });
        }

        [Fact]
        public async Task Should_CreateMassNotification_Throw_Exception_If_There_Is_No_Target_Notifier()
        {
            await Should.ThrowAsync<UserFriendlyException>(async () =>
            {
                await _notificationAppService.CreateMassNotification(new CreateMassNotificationInput()
                {
                    Message = "This is a test message",
                    Severity = NotificationSeverity.Info,
                    UserIds = new[] { 1L }
                });
            });
        }

        [Fact]
        public async Task Should_CreateMassNotification_Work()
        {
            await Should.NotThrowAsync(async () =>
            {
                await _notificationAppService.CreateMassNotification(new CreateMassNotificationInput()
                {
                    UserIds = new[] { AbpSession.GetUserId() },
                    TargetNotifiers = new[] { "MyTrainingV1231AngularDemo.Notifications.SmsRealTimeNotifier" },
                    Message = "This is a test message",
                    Severity = NotificationSeverity.Info
                });
            });
        }
    }
}