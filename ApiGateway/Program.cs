using Common.RabbitMQ;
using Common.RabbitMQ.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(x =>
{
    x.RegisterServicesFromAssemblyContaining<Program>();
});

builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
builder.Services.AddScoped<IPublisher, Publisher>();

builder.Services.AddSingleton<RabbitMQSetUp>();


var app = builder.Build();

using (var scope = app.Services.CreateScope()) 
{
    var setUp = scope.ServiceProvider.GetRequiredService<RabbitMQSetUp>();
    await setUp.Configure();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
