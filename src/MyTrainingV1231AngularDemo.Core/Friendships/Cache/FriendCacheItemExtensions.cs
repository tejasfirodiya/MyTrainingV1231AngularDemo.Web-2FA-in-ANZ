using System.Collections.Generic;
using System.Linq;

namespace MyTrainingV1231AngularDemo.Friendships.Cache
{
    public static class FriendCacheItemExtensions
    {
        public static bool ContainsFriend(this List<FriendCacheItem> items, FriendCacheItem item)
        {
            return items.Any(f => f.FriendTenantId == item.FriendTenantId && f.FriendUserId == item.FriendUserId);
        }
        public static int RemoveCachedFriend(this List<FriendCacheItem> items, FriendCacheItem item)
        {
            return items.RemoveAll(f => f.FriendTenantId == item.FriendTenantId && f.FriendUserId == item.FriendUserId);
        }

    }
}
