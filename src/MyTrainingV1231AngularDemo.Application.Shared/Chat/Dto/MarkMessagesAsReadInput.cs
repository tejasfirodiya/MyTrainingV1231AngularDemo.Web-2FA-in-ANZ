using Abp;

namespace MyTrainingV1231AngularDemo.Chat.Dto
{
    public class MarkAllUnreadMessagesOfUserAsReadInput
    {
        public int? TenantId { get; set; }

        public long UserId { get; set; }

        public UserIdentifier ToUserIdentifier()
        {
            return new UserIdentifier(TenantId, UserId);
        }
    }
}