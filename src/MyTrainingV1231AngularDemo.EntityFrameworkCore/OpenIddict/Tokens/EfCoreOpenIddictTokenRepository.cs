using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.EntityFrameworkCore;
using MyTrainingV1231AngularDemo.EntityFrameworkCore.Repositories;
using MyTrainingV1231AngularDemo.OpenIddict.Authorizations;
using OpenIddict.Abstractions;
using Z.EntityFramework.Plus;

namespace MyTrainingV1231AngularDemo.OpenIddict.Tokens
{
    public class EfCoreOpenIddictTokenRepository : MyTrainingV1231AngularDemoRepositoryBase<OpenIddictToken, Guid>,
        IOpenIddictTokenRepository
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EfCoreOpenIddictTokenRepository(
            IDbContextProvider<MyTrainingV1231AngularDemoDbContext> dbContextProvider,
            IUnitOfWorkManager unitOfWorkManager)
            : base(dbContextProvider)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        protected async Task<DbSet<OpenIddictToken>> GetDbSetAsync()
        {
            return (await GetDbContextAsync()).Set<OpenIddictToken>();
        }

        public virtual async Task DeleteManyByApplicationIdAsync(Guid applicationId, bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            var tokens = (await GetDbSetAsync())
                .Where(x => x.ApplicationId == applicationId);

            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await tokens.DeleteAsync(cancellationToken));
        }

        public virtual async Task DeleteManyByAuthorizationIdAsync(Guid authorizationId, bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            var tokens = (await GetDbSetAsync())
                .Where(x => x.AuthorizationId == authorizationId);

            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await tokens.DeleteAsync(cancellationToken));
        }

        public virtual async Task DeleteManyByAuthorizationIdsAsync(Guid[] authorizationIds, bool autoSave = false,
            CancellationToken cancellationToken = default)
        {
            var tokens = (await GetDbSetAsync())
                .Where(x => x.AuthorizationId != null && authorizationIds.Contains(x.AuthorizationId.Value));

            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await tokens.DeleteAsync(cancellationToken));
        }

        public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).Where(x => x.Subject == subject && x.ApplicationId == client)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync())
                    .Where(x => x.Subject == subject && x.ApplicationId == client && x.Status == status)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> FindAsync(string subject, Guid client, string status,
            string type, CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync())
                    .Where(x => x.Subject == subject && x.ApplicationId == client && x.Status == status && x.Type == type)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> FindByApplicationIdAsync(Guid applicationId,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).Where(x => x.ApplicationId == applicationId)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> FindByAuthorizationIdAsync(Guid authorizationId,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).Where(x => x.AuthorizationId == authorizationId)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<OpenIddictToken> FindByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.Id == id,
                    cancellationToken);
            });
        }

        public virtual async Task<OpenIddictToken> FindByReferenceIdAsync(string referenceId,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).FirstOrDefaultAsync(x => x.ReferenceId == referenceId,
                    cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> FindBySubjectAsync(string subject,
            CancellationToken cancellationToken = default)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await (await GetQueryableAsync()).Where(x => x.Subject == subject)
                    .ToListAsync(cancellationToken);
            });
        }

        public virtual async Task<List<OpenIddictToken>> ListAsync(int? count, int? offset,
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryableAsync();
            query = query.OrderBy(x => x.Id);

            if (offset.HasValue)
            {
                query = query.Skip(offset.Value);
            }

            if (count.HasValue)
            {
                query = query.Take(count.Value);
            }

            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () => await query.ToListAsync(cancellationToken));
        }

        public virtual async Task PruneAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                await (from token in await GetQueryableAsync()
                        join authorization in (await GetDbContextAsync()).Set<OpenIddictAuthorization>()
                            on token.AuthorizationId equals authorization.Id into tokenAuthorizations
                        from tokenAuthorization in tokenAuthorizations.DefaultIfEmpty()
                        where token.CreationDate < date
                        where (token.Status != OpenIddictConstants.Statuses.Inactive &&
                               token.Status != OpenIddictConstants.Statuses.Valid) ||
                              (tokenAuthorization != null &&
                               tokenAuthorization.Status != OpenIddictConstants.Statuses.Valid) ||
                              token.ExpirationDate < DateTime.UtcNow
                        select token)
                    .ExecuteDeleteAsync(cancellationToken);
            });
        }
    }
}