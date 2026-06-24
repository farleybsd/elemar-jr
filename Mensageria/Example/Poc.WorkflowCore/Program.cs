using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using Poc.WorkflowCore.Common;
using Poc.WorkflowCore.Domain.Orchestration.Infrastructure;
using WorkflowCore.Interface;
using AddEnrollment =
    Poc.WorkflowCore.Domain.Orchestration.Features.AddNewEnrollment;
using MainWork = Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow;
using NewFlow = Poc.WorkflowCore.Domain.Orchestration.Features.NewEnrollmentFlow.Steps;
//using CancelFlow = Poc.WorkflowCore.Domain.Orchestration.Features.CancelEnrollmentFlow;
var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Database");

builder.Services.AddDbContext<SubscriptionsDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddWorkflow(x => x.UsePostgreSQL(connectionString, true, true));
builder.Services.Configure<ServicesSettings>(builder.Configuration.GetSection("ServicesSettings"));
builder.Services
    .AddFastEndpoints()
    .SwaggerDocument();

// Workflow de nova matrícula
//builder.Services.AddSingleton<MainWork.MainWorkflow>();

builder.Services.AddScoped<NewFlow.AddEnrollmentStep>();
builder.Services.AddScoped<NewFlow.AddEnrollmentCompensationStep>();
builder.Services.AddScoped<NewFlow.ProcessPaymentStep>();
builder.Services.AddScoped<NewFlow.ProcessPaymentCompensationStep>();
builder.Services.AddScoped<NewFlow.ScheduleEvaluationStep>();
builder.Services.AddScoped<NewFlow.ScheduleEvaluationCompensationStep>();
builder.Services.AddScoped<AddEnrollment.Handler>(); ;

builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

// Registra a definição no catálogo do WorkflowCore
var workflowHost = app.Services.GetRequiredService<IWorkflowHost>();

workflowHost.RegisterWorkflow<
    MainWork.MainWorkflow,
    MainWork.NewEnrollmentFlowData>();

workflowHost.Start();

app.Lifetime.ApplicationStopping.Register(() =>
{
    workflowHost.Stop();
});

app.UseFastEndpoints();
app.UseSwaggerGen();

app.UseHttpsRedirection();
app.UseCors("Default");




app.Run();


