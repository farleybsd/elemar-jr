using IntelligentServiceFindZipCode.App.Crosscutting.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerWithVersioning();

builder.Services.AddViaCep(builder.Configuration.GetSection("Providers:ViaCep"));

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
    app.UseSwaggerWithUi();


app.UseEndPoints();
app.Run();

