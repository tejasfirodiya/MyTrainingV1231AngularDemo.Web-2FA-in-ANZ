using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Domain.Uow;
using Microsoft.Extensions.Logging;
using MyTrainingV1231AngularDemo.OpenIddict.Applications;
using MyTrainingV1231AngularDemo.OpenIddict.Tokens;
using OpenIddict.Abstractions;

namespace MyTrainingV1231AngularDemo.OpenIddict.Authorizations
{
    public class AbpOpenIddictAuthorizationStore : AbpOpenIddictStoreBase<IOpenIddictAuthorizationRepository>,
        IOpenIddictAuthorizationStore<OpenIddictAuthorizationModel>
    {
        protected IOpenIddictApplicationRepository ApplicationRepository { get; }
        protected IOpenIddictTokenRepository TokenRepository { get; }

        public AbpOpenIddictAuthorizationStore(
            IOpenIddictAuthorizationRepository repository,
            IUnitOfWorkManager unitOfWorkManager,
            IGuidGenerator guidGenerator,
            IOpenIddictApplicationRepository applicationRepository,
            IOpenIddictTokenRepository tokenRepository,
            IOpenIddictDbConcurrencyExceptionHandler concurrencyExceptionHandler)
            : base(repository, unitOfWorkManager, guidGenerator, concurrencyExceptionHandler)
        {
            ApplicationRepository = applicationRepository;
            TokenRepository = tokenRepository;
        }

        public virtual async ValueTask<long> CountAsync(CancellationToken cancellationToken)
        {
            return await Repository.CountAsync();
        }

        public virtual ValueTask<long> CountAsync<TResult>(
            Func<IQueryable<OpenIddictAuthorizationModel>, IQueryable<TResult>> query,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual async ValueTask CreateAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            await Repository.InsertAsync(authorization.ToEntity());

            authorization = (await Repository.FindByIdAsync(authorization.Id, cancellationToken)).ToModel();
        }

        public virtual async ValueTask DeleteAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            try
            {
                using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                       {
                           IsolationLevel = IsolationLevel.RepeatableRead,
                           IsTransactional = true,
                           Scope = TransactionScopeOption.RequiresNew
                       }))
                {
                    await TokenRepository.DeleteManyByAuthorizationIdAsync(authorization.Id,
                        cancellationToken: cancellationToken);

                    await Repository.DeleteAsync(authorization.Id);

                    await uow.CompleteAsync();
                }
            }
            catch (AbpDbConcurrencyException e)
            {
                Logger.LogError(e, e.Message);
                await ConcurrencyExceptionHandler.HandleAsync(e);
                throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));

            var authorizations = await Repository.FindAsync(subject, Guid.Parse(client), cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client,
            string status, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));
            Check.NotNullOrEmpty(status, nameof(status));

            var authorizations = await Repository.FindAsync(subject, Guid.Parse(client), status,
                cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client,
            string status, string type, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));
            Check.NotNullOrEmpty(status, nameof(status));
            Check.NotNullOrEmpty(type, nameof(type));

            var authorizations = await Repository.FindAsync(subject, Guid.Parse(client), status, type,
                cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindAsync(string subject, string client,
            string status, string type, ImmutableArray<string> scopes,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));
            Check.NotNullOrEmpty(client, nameof(client));
            Check.NotNullOrEmpty(status, nameof(status));
            Check.NotNullOrEmpty(type, nameof(type));

            var authorizations = await Repository.FindAsync(subject, Guid.Parse(client), status, type,
                cancellationToken);

            foreach (var authorization in authorizations)
            {
                if (new HashSet<string>(await GetScopesAsync(authorization.ToModel(), cancellationToken),
                        StringComparer.Ordinal).IsSupersetOf(scopes))
                {
                    yield return authorization.ToModel();
                }
            }
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindByApplicationIdAsync(string identifier,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            var authorizations = await Repository.FindByApplicationIdAsync(Guid.Parse(identifier), cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual async ValueTask<OpenIddictAuthorizationModel> FindByIdAsync(string identifier,
            CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(identifier, nameof(identifier));

            return (await Repository.FindByIdAsync(Guid.Parse(identifier), cancellationToken))
                .ToModel();
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> FindBySubjectAsync(string subject,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            Check.NotNullOrEmpty(subject, nameof(subject));

            var authorizations = await Repository.FindBySubjectAsync(subject, cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual ValueTask<string> GetApplicationIdAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return new ValueTask<string>(authorization.ApplicationId?.ToString());
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(
            Func<IQueryable<OpenIddictAuthorizationModel>, TState, IQueryable<TResult>> query, TState state,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return authorization.CreationDate is null
                ? new ValueTask<DateTimeOffset?>(result: null)
                : new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(authorization.CreationDate.Value,
                    DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetIdAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return new ValueTask<string>(authorization.Id.ToString());
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(
            OpenIddictAuthorizationModel authorization, CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            if (string.IsNullOrEmpty(authorization.Properties))
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary
                    .Create<string, JsonElement>());
            }

            using (var document = JsonDocument.Parse(authorization.Properties))
            {
                var builder = ImmutableDictionary.CreateBuilder<string, JsonElement>();

                foreach (var property in document.RootElement.EnumerateObject())
                {
                    builder[property.Name] = property.Value.Clone();
                }

                return new ValueTask<ImmutableDictionary<string, JsonElement>>(builder.ToImmutable());
            }
        }

        public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            if (string.IsNullOrEmpty(authorization.Scopes))
            {
                return new ValueTask<ImmutableArray<string>>(ImmutableArray.Create<string>());
            }

            using (var document = JsonDocument.Parse(authorization.Scopes))
            {
                var builder = ImmutableArray.CreateBuilder<string>(document.RootElement.GetArrayLength());

                foreach (var element in document.RootElement.EnumerateArray())
                {
                    var value = element.GetString();
                    if (string.IsNullOrEmpty(value))
                    {
                        continue;
                    }

                    builder.Add(value);
                }

                return new ValueTask<ImmutableArray<string>>(builder.ToImmutable());
            }
        }

        public virtual ValueTask<string> GetStatusAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return new ValueTask<string>(authorization.Status);
        }

        public virtual ValueTask<string> GetSubjectAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return new ValueTask<string>(authorization.Subject);
        }

        public virtual ValueTask<string> GetTypeAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            return new ValueTask<string>(authorization.Type);
        }

        public virtual ValueTask<OpenIddictAuthorizationModel> InstantiateAsync(CancellationToken cancellationToken)
        {
            return new ValueTask<OpenIddictAuthorizationModel>(new OpenIddictAuthorizationModel
            {
                Id = GuidGenerator.Create()
            });
        }

        public virtual async IAsyncEnumerable<OpenIddictAuthorizationModel> ListAsync(int? count, int? offset,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var authorizations = await Repository.ListAsync(count, offset, cancellationToken);
            foreach (var authorization in authorizations)
            {
                yield return authorization.ToModel();
            }
        }

        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(
            Func<IQueryable<OpenIddictAuthorizationModel>, TState, IQueryable<TResult>> query, TState state,
            CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual async ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            using (var uow = UnitOfWorkManager.Begin(new UnitOfWorkOptions()
                   {
                       Scope = TransactionScopeOption.RequiresNew,
                       IsTransactional = true,
                       IsolationLevel = IsolationLevel.RepeatableRead
                   }))
            {
                var date = threshold.UtcDateTime;
                await Repository.PruneAsync(date, cancellationToken);
                await uow.CompleteAsync();
            }
        }

        public virtual async ValueTask SetApplicationIdAsync(OpenIddictAuthorizationModel authorization,
            string identifier, CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            if (!string.IsNullOrEmpty(identifier))
            {
                var application = await ApplicationRepository.GetAsync(Guid.Parse(identifier));
                authorization.ApplicationId = application.Id;
            }
            else
            {
                authorization.ApplicationId = null;
            }
        }

        public virtual ValueTask SetCreationDateAsync(OpenIddictAuthorizationModel authorization, DateTimeOffset? date,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            authorization.CreationDate = date?.UtcDateTime;

            return default;
        }

        public virtual ValueTask SetPropertiesAsync(OpenIddictAuthorizationModel authorization,
            ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (properties is null || properties.IsEmpty)
            {
                authorization.Properties = null;
                return default;
            }

            authorization.Properties = WriteStream(writer =>
            {
                writer.WriteStartObject();
                foreach (var property in properties)
                {
                    writer.WritePropertyName(property.Key);
                    property.Value.WriteTo(writer);
                }

                writer.WriteEndObject();
            });

            return default;
        }

        public virtual ValueTask SetScopesAsync(OpenIddictAuthorizationModel authorization,
            ImmutableArray<string> scopes, CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            if (scopes.IsDefaultOrEmpty)
            {
                authorization.Scopes = null;
                return default;
            }

            authorization.Scopes = WriteStream(writer =>
            {
                writer.WriteStartArray();
                foreach (var scope in scopes)
                {
                    writer.WriteStringValue(scope);
                }

                writer.WriteEndArray();
            });

            return default;
        }

        public virtual ValueTask SetStatusAsync(OpenIddictAuthorizationModel authorization, string status,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            authorization.Status = status;

            return default;
        }

        public virtual ValueTask SetSubjectAsync(OpenIddictAuthorizationModel authorization, string subject,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            authorization.Subject = subject;

            return default;
        }

        public virtual ValueTask SetTypeAsync(OpenIddictAuthorizationModel authorization, string type,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            authorization.Type = type;

            return default;
        }

        public virtual async ValueTask UpdateAsync(OpenIddictAuthorizationModel authorization,
            CancellationToken cancellationToken)
        {
            Check.NotNull(authorization, nameof(authorization));

            var entity = await Repository.GetAsync(authorization.Id);

            try
            {
                await Repository.UpdateAsync(authorization.ToEntity(entity));
            }
            catch (AbpDbConcurrencyException e)
            {
                Logger.LogError(e, e.Message);
                await ConcurrencyExceptionHandler.HandleAsync(e);
                throw new OpenIddictExceptions.ConcurrencyException(e.Message, e.InnerException);
            }

            authorization = (await Repository.FindByIdAsync(entity.Id, cancellationToken: cancellationToken)).ToModel();
        }
    }
}