// ----------------------------------------------------------------------------------
// Copyright (c) The Standard Organization: A coalition of the Good-Hearted Engineers
// ----------------------------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tracker.Core.Api.Brokers.DateTimes;
using Tracker.Core.Api.Brokers.Loggings;
using Tracker.Core.Api.Brokers.Storages;
using Tracker.Core.Api.Services.Foundations.Categories;
using Tracker.Core.Api.Services.Foundations.Transactions;
using Tracker.Core.Api.Services.Foundations.Users;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<StorageBroker>();
AddBrokers(builder.Services);
AddFoundationServices(builder.Services);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

static void AddBrokers(IServiceCollection services)
{
    services.AddTransient<IStorageBroker, StorageBroker>();
    services.AddTransient<ILoggingBroker, LoggingBroker>();
    services.AddTransient<IDateTimeBroker, DateTimeBroker>();
}

static void AddFoundationServices(IServiceCollection services)
{
    services.AddTransient<IUserService, UserService>();
    services.AddTransient<ICategoryService, CategoryService>();
    services.AddTransient<ITransactionService, TransactionService>();
}