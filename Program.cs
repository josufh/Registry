using Registry.Middleware;
using Registry.Models;
using Registry.Services.Digestion;
using Registry.Services.Repository;
using Registry.Services.Storage;
using Registry.Services.Uploads;
using Registry.Services.Validation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBlobStorage, BlobStorage>();
builder.Services.AddSingleton<IDigester, Digester>();
builder.Services.AddSingleton<IValidationService, ValidationService>();
builder.Services.AddSingleton<IUploadService, UploadService>();
builder.Services.AddSingleton<IRepositoryService, RepositoryService>();

builder.Services.AddScoped<Blob>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<NamespaceEscapingMiddleware>();
app.UseBlobInterceptionMiddleware();

app.MapControllers();

app.Run();