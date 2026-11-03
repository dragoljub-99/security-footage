using SecurityFootage.Infrastructure.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpClient<SasUrlClient>(client =>
{
    string baseUrl = builder.Configuration["GenerateSasUrl:BaseUrl"]
                     ?? throw new InvalidOperationException("Base url is not configured");

    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers();
var app = builder.Build();

app.MapControllers();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();


app.Run();
