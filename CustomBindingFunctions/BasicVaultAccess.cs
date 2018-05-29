using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.KeyVault;
using System.Threading.Tasks;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomBindingFunctions
{
    public static class BasicVaultAccess
    {
        [FunctionName("BasicVaultAccess")]
        public static async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, TraceWriter log)
        {
            try
            {
                log.Info("Starting...");
                var azureServiceTokenProvider = new AzureServiceTokenProvider();

                log.Info("Building kvClient");
                var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

                log.Info("Retrieving Value");
                string secretValue = (await kvClient.GetSecretAsync("https://bindingsecrets.vault.azure.net/Secrets/AccessKey")).Value;

                log.Info("Returning value...");
                return (ActionResult)new OkObjectResult($"{secretValue}");
            }
            catch (Exception ex)
            {
                log.Error($"Error: {ex.Message}", ex);

                return new BadRequestObjectResult(ex);
            }
        }
    }
}
