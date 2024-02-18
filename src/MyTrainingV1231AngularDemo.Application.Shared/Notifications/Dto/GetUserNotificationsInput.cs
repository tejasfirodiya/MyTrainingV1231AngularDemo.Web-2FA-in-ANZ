using System;
using Abp.Notifications;
using MyTrainingV1231AngularDemo.Dto;

namespace MyTrainingV1231AngularDemo.Notifications.Dto
{
    public class GetUserNotificationsInput : PagedInputDto
    {
        public UserNotificationState? State { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}