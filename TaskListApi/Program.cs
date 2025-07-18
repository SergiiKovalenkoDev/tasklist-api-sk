using Cortex.Mediator.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using TaskListApi.Database;
using TaskListApi.Dtos;
using TaskListApi.Middlewares;
using TaskListApi.Repositories;
using TaskListApi.Repositories.Interfaces;
using TaskListApi.Services;
using TaskListApi.Services.Interfaces;
using TaskListApi.Validators;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<CreateTaskListDto>, CreateTaskListDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskListDto>, UpdateTaskListDtoValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<ITaskListService, TaskListService>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCortexMediator(
    builder.Configuration,
    new[] { typeof(Program) } 
);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();


app.Run();

