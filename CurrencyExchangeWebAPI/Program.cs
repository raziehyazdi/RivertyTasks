using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RivertyTasks;
using RivertyTasks.Data;
using RivertyTasks.Models;
using RivertyTasks.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddControllers();


builder.Services.AddDbContext<ExchangeRateDbContext>(options =>
    options.UseInMemoryDatabase("RivertyExchangeInMemoryDB")); 

builder.Services.AddHttpClient<ExchangeRateService>();
builder.Services.AddScoped<ExchangeRateService>();
TestDataCreator.Initialize(builder.Configuration);

var app = builder.Build();

await TestDataCreator.DataCreatorAsync(app.Services);

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();

app.Run();

