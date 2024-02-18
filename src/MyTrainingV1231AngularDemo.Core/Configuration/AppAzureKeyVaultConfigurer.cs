using System;
using Microsoft.Extensions.Configuration;
using Azure.Identity;

namespace MyTrainingV1231AngularDemo.Configuration
{
    public class AppAzureKeyVaultConfigurer
    {
        public void Configure(IConfigurationBuilder builder, IConfigurationRoot config)
        {
            var azureKeyVaultConfiguration = config.GetSection("Configuration:AzureKeyVault")
                .Get<AzureKeyVaultConfiguration>();

            if (azureKeyVaultConfiguration == null || !azureKeyVaultConfiguration.IsEnabled)
            {
                return;
            }

            var azureKeyVaultUrl = $"https://{azureKeyVaultConfiguration.KeyVaultName}.vault.azure.net/";
            builder.AddAzureKeyVault(new Uri(azureKeyVaultUrl), new ClientSecretCredential(
                azureKeyVaultConfiguration.TenantId, 
                azureKeyVaultConfiguration.ClientId, 
                azureKeyVaultConfiguration.ClientSecret)
            );
        }
    }
}