using Abp;
using Abp.Domain.Repositories;
using Abp.Runtime.Caching;
using MyTrainingV1231AngularDemo.Chat;
using System.Linq;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using MyTrainingV1231AngularDemo.Authorization.Users;

namespace MyTrainingV1231AngularDemo.Friendships.Cache
{
    public class UserFriendsCache : IUserFriendsCache, ISingletonDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IRepository<Friendship, long> _friendshipRepository;
        private readonly IRepository<ChatMessage, long> _chatMessageRepository;
        private readonly ITenantCache _tenantCache;
        private readonly UserStore _userStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        private readonly object _syncObj = new object();

        public UserFriendsCache(
            ICacheManager cacheManager,
            IRepository<Friendship, long> friendshipRepository,
            IRepository<ChatMessage, long> chatMessageRepository,
            ITenantCache tenantCache,
            IUnitOfWorkManager unitOfWorkManager,
            UserStore userStore)
        {
            _cacheManager = cacheManager;
            _friendshipRepository = friendshipRepository;
            _chatMessageRepository = chatMessageRepository;
            _tenantCache = tenantCache;
            _unitOfWorkManager = unitOfWorkManager;
            _userStore = userStore;
        }

        public virtual UserWithFriendsCacheItem GetCacheItem(UserIdentifier userIdentifier)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _cacheManager
                    .GetCache(FriendCacheItem.CacheName)
                    .AsTyped<string, UserWithFriendsCacheItem>()
                    .Get(userIdentifier.ToUserIdentifierString(), f => GetUserFriendsCacheItemInternal(userIdentifier));
            });
        }

        public virtual UserWithFriendsCacheItem GetCacheItemOrNull(UserIdentifier userIdentifier)
        {
            return _cacheManager
                .GetCache(FriendCacheItem.CacheName)
                .AsTyped<string, UserWithFriendsCacheItem>()
                .GetOrDefault(userIdentifier.ToUserIdentifierString());
        }

        public virtual void ResetUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var user = GetCacheItemOrNull(userIdentifier);
                if (user == null)
                {
                    return;
                }

                lock (_syncObj)
                {
                    var friend = user.Friends.FirstOrDefault(
                        f => f.FriendUserId == friendIdentifier.UserId &&
                             f.FriendTenantId == friendIdentifier.TenantId
                    );

                    if (friend == null)
                    {
                        return;
                    }

                    friend.UnreadMessageCount = 0;
                    UpdateUserOnCache(userIdentifier, user);
                }
            });
        }

        public virtual void IncreaseUnreadMessageCount(UserIdentifier userIdentifier, UserIdentifier friendIdentifier,
            int change)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var user = GetCacheItemOrNull(userIdentifier);
                if (user == null)
                {
                    return;
                }

                lock (_syncObj)
                {
                    var friend = user.Friends.FirstOrDefault(
                        f => f.FriendUserId == friendIdentifier.UserId &&
                             f.FriendTenantId == friendIdentifier.TenantId
                    );

                    if (friend == null)
                    {
                        return;
                    }

                    friend.UnreadMessageCount += change;
                    UpdateUserOnCache(userIdentifier, user);
                }
            });
        }

        public void AddFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                if (!user.Friends.ContainsFriend(friend))
                {
                    user.Friends.Add(friend);
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        public void RemoveFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                if (user.Friends.ContainsFriend(friend))
                {
                    user.Friends.RemoveCachedFriend(friend);
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        public void UpdateFriend(UserIdentifier userIdentifier, FriendCacheItem friend)
        {
            var user = GetCacheItemOrNull(userIdentifier);
            if (user == null)
            {
                return;
            }

            lock (_syncObj)
            {
                var existingFriendIndex = user.Friends.FindIndex(
                    f => f.FriendUserId == friend.FriendUserId &&
                         f.FriendTenantId == friend.FriendTenantId
                );

                if (existingFriendIndex >= 0)
                {
                    user.Friends[existingFriendIndex] = friend;
                    UpdateUserOnCache(userIdentifier, user);
                }
            }
        }

        protected virtual UserWithFriendsCacheItem GetUserFriendsCacheItemInternal(UserIdentifier userIdentifier)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                var tenancyName = userIdentifier.TenantId.HasValue
                    ? _tenantCache.GetOrNull(userIdentifier.TenantId.Value)?.TenancyName
                    : null;

                using (_unitOfWorkManager.Current.SetTenantId(userIdentifier.TenantId))
                {
                    var friendCacheItems = _friendshipRepository.GetAll()
                        .Where(friendship => friendship.UserId == userIdentifier.UserId)
                        .Select(friendship => new FriendCacheItem
                        {
                            FriendUserId = friendship.FriendUserId,
                            FriendTenantId = friendship.FriendTenantId,
                            State = friendship.State,
                            FriendUserName = friendship.FriendUserName,
                            FriendTenancyName = friendship.FriendTenancyName,
                            FriendProfilePictureId = friendship.FriendProfilePictureId,
                            UnreadMessageCount = _chatMessageRepository.GetAll().Count(cm =>
                                cm.ReadState == ChatMessageReadState.Unread &&
                                cm.UserId == userIdentifier.UserId &&
                                cm.TenantId == userIdentifier.TenantId &&
                                cm.TargetUserId == friendship.FriendUserId &&
                                cm.TargetTenantId == friendship.FriendTenantId &&
                                cm.Side == ChatSide.Receiver)
                        }).ToList();

                    var user = _userStore.FindById(userIdentifier.UserId.ToString());

                    return new UserWithFriendsCacheItem
                    {
                        TenantId = userIdentifier.TenantId,
                        UserId = userIdentifier.UserId,
                        TenancyName = tenancyName,
                        UserName = user.UserName,
                        ProfilePictureId = user.ProfilePictureId,
                        Friends = friendCacheItems
                    };
                }
            });
        }

        private void UpdateUserOnCache(UserIdentifier userIdentifier, UserWithFriendsCacheItem user)
        {
            _cacheManager.GetCache(FriendCacheItem.CacheName).Set(userIdentifier.ToUserIdentifierString(), user);
        }
    }
}
