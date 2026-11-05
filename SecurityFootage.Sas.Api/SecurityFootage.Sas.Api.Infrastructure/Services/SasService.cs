using Apache.Arrow;
using Microsoft.VisualBasic;
using Azure.Identity;
using Azure.Storage.Blobs;
using System.Diagnostics.CodeAnalysis;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using DownloadFootageSas.Infrastructure.Interfaces;

namespace DownloadFootageSas.Infrastructure.Services
{
    public class SasService : ISasService
    {
        public async Task<string> GenerateDownloadUrlAsync(string blobName, CancellationToken ct = default)
        {
            var credential =  BuildAzureConnection(ct);
            var accountName = "scftacc";
            var containerName = "videos";
            
            var serviceClient = new BlobServiceClient(
                new Uri ($"https://{accountName}.blob.core.windows.net"),
                credential);

            var blobClient = serviceClient.GetBlobContainerClient(containerName)
                                          .GetBlobClient(blobName);
            
            DateTimeOffset now = DateTimeOffset.UtcNow;
            DateTimeOffset startsOn = now.AddMinutes(-5);
            DateTimeOffset expireOn = now.AddMinutes(5);

            var options = new BlobGetUserDelegationKeyOptions(expireOn.AddMinutes(5))
            {
                StartsOn = startsOn
            };

            var delegationKey = (await serviceClient.GetUserDelegationKeyAsync(options, ct)).Value; 

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "r",
                StartsOn = startsOn,
                ExpiresOn = expireOn,
                Protocol = SasProtocol.Https,
                ContentDisposition = "attachment; filename=\"video.mp4\"",
                ContentType = "video/mp4"
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasParameters = sasBuilder.ToSasQueryParameters(delegationKey, accountName);

            var uriBuilder = new BlobUriBuilder(blobClient.Uri)
            {
                Sas = sasParameters
            };

            return uriBuilder.ToUri().AbsoluteUri;
        }

        public static ClientSecretCredential BuildAzureConnection(CancellationToken ct = default)
        {
            var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID")
            ?? throw new InvalidOperationException("Tenant ID is missing");

            var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")
            ?? throw new InvalidOperationException("CLient ID is missing");

            var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET")
            ?? throw new InvalidOperationException("Client secret is missing");

            return new ClientSecretCredential(tenantId, clientId, clientSecret);
        }
    }
}