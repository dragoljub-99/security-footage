using SecurityFootage.Domain.Entities;

namespace SecurityFootage.Domain.Results
{
    public sealed record CreateUserResult(RequestResult Result, User? User);
}