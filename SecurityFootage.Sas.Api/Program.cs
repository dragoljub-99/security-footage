using DownloadFootageSas.Infrastructure.Services;
using DownloadFootageSas.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddScoped<ISasService, SasService>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/footage/downloadUrl/{blobName}", async (string blobName, ISasService _sasService, CancellationToken ct) =>
{
    string url = await _sasService.GenerateDownloadUrlAsync(blobName, ct);

    return Results.Ok(new {downloadUrl = url});
});

app.Run();


