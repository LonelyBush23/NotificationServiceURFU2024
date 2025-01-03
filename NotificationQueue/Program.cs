using NotificationQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Common.RabbitMQ;
using NotificationQueue.Domain.Repositories;
using NotificationQueue.Infrastructure.Repositories;

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

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddHostedService<RabbitMqBackgroundService>();

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
    _ = x.MapControllers();
});

app.UseHttpsRedirection();
app.Run();