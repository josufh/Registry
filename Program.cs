using Registry.Middleware;
using Registry.Models;
using Registry.Services.Digestion;
using Registry.Services.Storage;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDigester, Digester>();
builder.Services.AddScoped<IBlobStorage, BlobStorage>();
builder.Services.AddScoped<Blob>();

WebApplication app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseBlobInterceptionMiddleware();

app.MapControllers();

app.Run();