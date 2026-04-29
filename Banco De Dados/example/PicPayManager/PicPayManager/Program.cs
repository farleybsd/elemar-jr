using PicPayManager.Crosscutting.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerWithVersioning();
builder.Services.AddSqlServerDb(builder.Configuration);
builder.Services.AddRepository();

var app = builder.Build();
MigrationExtensions.ApplyMigrations(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerWithUi();
}

app.UseHttpsRedirection();
app.UseEndPoints();

app.Run();