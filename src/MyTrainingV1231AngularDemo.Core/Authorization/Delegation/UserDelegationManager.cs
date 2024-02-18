using System;
using System.Threading.Tasks;
using Abp;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace MyTrainingV1231AngularDemo.Authorization.Delegation
{
    public class UserDelegationManager : MyTrainingV1231AngularDemoServiceBase, IUserDelegationManager
    {
        private readonly IRepository<UserDelegation, long> _userDelegationRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UserDelegationManager(IRepository<UserDelegation, long> userDelegationRepository, IUnitOfWorkManager unitOfWorkManager)
        {
            _userDelegationRepository = userDelegationRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }
    
        public async Task<bool> HasActiveDelegationAsync(long sourceUserId, long targetUserId)
        {
            var activeUserDelegationExpression = new ActiveUserDelegationSpecification(sourceUserId, targetUserId)
                .ToExpression();

            var activeDelegation = await _userDelegationRepository.FirstOrDefaultAsync(activeUserDelegationExpression);

            return activeDelegation != null;
        }

        public bool HasActiveDelegation(long sourceUserId, long targetUserId)
        {
            UserDelegation activeDelegation;
            using (var uow = _unitOfWorkManager.Begin())
            {
                var activeUserDelegationExpression = new ActiveUserDelegationSpecification(sourceUserId, targetUserId)
                    .ToExpression();

                activeDelegation = _userDelegationRepository.FirstOrDefault(activeUserDelegationExpression);
                uow.Complete();
            }

            return activeDelegation != null;
        }

        public async Task RemoveDelegationAsync(long userDelegationId, UserIdentifier currentUser)
        {
            var delegation = await _userDelegationRepository.FirstOrDefaultAsync(e =>
                e.Id == userDelegationId && e.SourceUserId == currentUser.UserId
            );

            if (delegation == null)
            {
                throw new Exception("Only source user can delete a user delegation !");
            }

            await _userDelegationRepository.DeleteAsync(delegation);
        }

        public async Task<UserDelegation> GetAsync(long userDelegationId)
        {
            return await _userDelegationRepository.GetAsync(userDelegationId);
        }
    }
}