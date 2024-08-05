using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using UKHO.D365CallbackDistributorStub.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "UKHO.D365CallbackDistributorStub.API", Version = "v1" });
});
builder.Services.AddScoped<CallbackService>();
builder.Services.AddScoped<DistributionService>();


var app = builder.Build();

string? path = Directory.GetCurrentDirectory();

// Add a logger to the application
var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddFile($"{path}\\Logs\\Log.txt");
});

var logger = loggerFactory.CreateLogger<Program>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "UKHO.D365CallbackDistributorStub.API v1"));
}


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
