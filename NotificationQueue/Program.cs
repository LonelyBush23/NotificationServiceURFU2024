using NotificationQueue.Domain.Repositories;
using NotificationQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NotificationQueue.Infrastructure.Repositories;
using NotificationQueue.Infrastructure.RabbitMQ;
using NotificationQueue.Infrastructure.RabbitMQ.Base;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Services.AddDbContext<ServerDbContext>(config =>
{
    config.UseNpgsql(builder.Configuration.GetConnectionString("Server"));
    config.EnableSensitiveDataLogging();
});

builder.Services.RegisterRepository<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<IRabbitMqPublisher, RabbitMqPublisher>();
builder.Services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
builder.Services.AddScoped<IMessageQueue, MessageQueue>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();
app.UseRouting();
app.UseEndpoints(x =>
{
    x.MapControllers();
});

app.UseHttpsRedirection();
app.Run();