using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MyTrainingV1231AngularDemo.Configuration;
using OpenIddict.Abstractions;

namespace MyTrainingV1231AngularDemo.Web.OpenIddict
{
    public class OpenIdDictDataSeedWorker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IAppConfigurationAccessor _configuration;

        public OpenIdDictDataSeedWorker(IServiceProvider serviceProvider, IAppConfigurationAccessor configuration)
        {
            _serviceProvider = serviceProvider;
            _configuration = configuration;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (_configuration.Configuration["OpenIddict:IsEnabled"] == "false")
            {
                return;
            }

            foreach (var child in _configuration.Configuration.GetSection("OpenIddict:Applications").GetChildren())
            {
                await SaveScopes(child);
                await SaveApplications(child);
            }
        }

        private async Task SaveApplications(IConfigurationSection child)
        {
            var clientId = child["ClientId"];

            using var scope = _serviceProvider.CreateScope();
            var applicationManager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await applicationManager.FindByClientIdAsync(clientId) == null)
            {
                var application = new OpenIddictApplicationDescriptor
                {
                    ClientId = clientId,
                    ClientSecret = child["ClientSecret"],
                    ConsentType = child["ConsentType"],
                    DisplayName = child["DisplayName"]
                };

                AddItemsFromConfiguration(child, "Scopes",
                    (s) => application.Permissions.Add(OpenIddictConstants.Permissions.Prefixes.Scope + s)
                );

                AddItemsFromConfiguration(child, "Permissions",
                    (permission) => application.Permissions.Add(permission)
                );

                AddItemsFromConfiguration(child, "RedirectUris",
                    (uri) => application.RedirectUris.Add(new Uri(uri))
                );

                AddItemsFromConfiguration(child, "PostLogoutRedirectUris",
                    (uri) => application.PostLogoutRedirectUris.Add(new Uri(uri))
                );

                await applicationManager.CreateAsync(application);
            }
        }

        private async Task SaveScopes(IConfigurationSection child)
        {
            using var scope = _serviceProvider.CreateScope();

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            var unitOfWorkManager = scope.ServiceProvider.GetRequiredService<IUnitOfWorkManager>();

            var scopes = child.GetSection("Scopes").GetChildren().Select(c => c.Value).ToList();
            
            foreach (var scopeName in scopes)
            {
                await unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                {
                    if (await scopeManager.FindByNameAsync(scopeName) == null)
                    {
                        await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                        {
                            Name = scopeName,
                            DisplayName = scopeName,
                            Resources =
                            {
                                scopeName
                            },
                        });
                    }
                });
            }
        }

        private void AddItemsFromConfiguration(IConfigurationSection configSection, string key,
            Action<string> itemAdder)
        {
            var items = configSection.GetSection(key).GetChildren().Select(c => c.Value).ToList();
            foreach (var item in items)
            {
                itemAdder(item);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}