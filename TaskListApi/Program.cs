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

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<CreateTaskListDto>, CreateTaskListDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateTaskListDto>, UpdateTaskListDtoValidator>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<ITaskListRepository, TaskListRepository>();
builder.Services.AddScoped<ITaskListService, TaskListService>();

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

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

