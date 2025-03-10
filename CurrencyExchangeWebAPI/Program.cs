using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RivertyTasks;
using RivertyTasks.Data;
using RivertyTasks.Models;
using RivertyTasks.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Currency Exchange API",
        Version = "v1",
        Description = "API for currency conversion and exchange history"
    });
});

builder.Services.AddDbContext<ExchangeRateDbContext>(options =>
    options.UseInMemoryDatabase("RivertyExchangeInMemoryDB"));

builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddScoped<ExchangeRateService>();

TestDataCreator.Initialize(builder.Configuration);

var app = builder.Build();

await TestDataCreator.DataCreatorAsync(app.Services);

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Currency Exchange API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();

app.MapControllers();

app.Run();