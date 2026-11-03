namespace SecurityFootage.Api.Models
{
    public sealed record UserCreatedDto(long Id, string Nickname, DateTime CreatedAtUtc);
}