using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Sql;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SecurityFootage.Infrastructure.Services;
using SecurityFootage.DatabaseProvisioning.Exceptions;
using SecurityFootage.Infrastructure.Abstractions;



namespace SecurityFootage.DatabaseProvisioning
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var tenantId = Environment.GetEnvironmentVariable("AZURE_TENANT_ID")
            ?? throw new AzureTenantIdNullException("Azure tenant id is missing!");

            var clientId = Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")
            ?? throw new AzureClientIdNullException("Azure client id is missing!");

            var clientSecret = Environment.GetEnvironmentVariable("AZURE_CLIENT_SECRET")
            ?? throw new AzureClientSecretNullException("Azure client secret is missing!"); 

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);

            var armClient = new ArmClient(credential);

            SubscriptionResource subscription = await armClient.GetDefaultSubscriptionAsync();           

            ResourceGroupResource resourceGroup = await subscription.GetResourceGroups()
                                                                    .GetAsync("security-footage-rg");

            SqlServerResource sqlServer = await resourceGroup.GetSqlServers()
                                                             .GetAsync("ledg-db-server");

            SqlDatabaseCollection databases = sqlServer.GetSqlDatabases();

            var databaseResponse = await databases.GetIfExistsAsync("Ledgers", cancellationToken:default);

            if (databaseResponse.HasValue)
            {
                Console.WriteLine("Database with Ledgers name exist, checking tables..");
                
                SqlDatabaseResource database = databaseResponse.Value!;
                SqlDatabaseSchemaResource schema = await database!.GetSqlDatabaseSchemaAsync("dbo", 
                                                                                cancellationToken:default);
                SqlDatabaseTableCollection tables = schema.GetSqlDatabaseTables();
                
                bool hasAnyTable = false;

                await foreach(SqlDatabaseTableResource resource in tables.GetAllAsync(cancellationToken: default))
                {
                    hasAnyTable = true;
                    break;
                }

                if (hasAnyTable)
                {
                    Console.WriteLine("Database schema exist, press enter to close");
                    Console.ReadLine();
                }
                else
                {
                    await InitializeDatabaseAsync(args);

                    Console.WriteLine("Tables created, database is ready now, press enter to close...");                   
                    
                }
            }          
            else
            {
                Console.WriteLine("Database dont exist, wait for the creation...");
                var databaseData = new SqlDatabaseData(AzureLocation.ItalyNorth);

                ArmOperation<SqlDatabaseResource> operation = await databases.CreateOrUpdateAsync(
                                                                    Azure.WaitUntil.Completed,
                                                                    "Ledgers", databaseData);

                SqlDatabaseResource database = operation.Value;
                Console.Clear();
                Console.WriteLine($"Database {database.Data.Name} was created succesfully!\n");
                Console.WriteLine("Creating base schema...");

                await InitializeDatabaseAsync(args);

                Console.Clear();
                Console.WriteLine("Database schema created successfully");
                Console.WriteLine("Press enter to close");
                Console.ReadKey();
                
            }
        }
        public static async Task InitializeDatabaseAsync(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);

            using var host = builder.Build();
            using var scope = host.Services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
            await service.EnsureDatabaseSchemaAsync();
        }
    }
}