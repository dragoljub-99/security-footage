namespace SecurityFootage.Infrastructure.Abstractions
{
    public interface IDatabaseInitializer
    {
         Task EnsureDatabaseSchemaAsync(CancellationToken ct = default);
    }
}