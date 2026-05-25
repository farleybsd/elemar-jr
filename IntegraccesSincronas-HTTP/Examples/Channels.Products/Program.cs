using Channels.Products.Crosscutting;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSwaggerWithVersioning();
builder.Services.AddBackgroundService();
builder.Services.AddChannels();
builder.Services.AddServices();
builder.Services.AddRepository();
builder.Services.AddChannels();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
    app.UseSwaggerWithUi();


app.UseEndpoints();

await app.RunAsync();

