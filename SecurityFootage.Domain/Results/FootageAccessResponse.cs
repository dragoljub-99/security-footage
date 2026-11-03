namespace SecurityFootage.Domain.Results
{
    public sealed record FootageAccessResponse(RequestResult Result, string? SasUrl);
}