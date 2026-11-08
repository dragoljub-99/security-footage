namespace SecurityFootage.Sas.Infrastructure.Interfaces
{
    public interface ISasService
    {
        Task<string> GenerateDownloadUrlAsync(string blobName, CancellationToken ct = default);
    }
}