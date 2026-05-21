using Api.IntelligentServiceFindZipCode.Crosscutting;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilogLogging(builder.Configuration);

builder.Host.UseSerilog();

builder.Services.AddIntelligentServiceFindZipCodeResiliencePolicies();
builder.Services.AddViaCepServiceOptions();
builder.Services.AddSwaggerWithVersioning();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
    app.UseSwaggerWithUi();

app.UseEndpoints();
app.WellCome();
await app.RunAsync();

