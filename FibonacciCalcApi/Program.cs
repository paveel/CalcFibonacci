using EasyNetQ;
using FibonacciCalc;
using FibonacciCalcApi;
using FibonacciCalcApp;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((hostingContext, loggerConfig) =>
            loggerConfig.ReadFrom.Configuration(hostingContext.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton(builder.Configuration.GetSection(MessageBrokerOptions.Name)
                                                    .Get<MessageBrokerOptions>());
builder.Services.Configure<MessageBrokerOptions>(opt =>
                    builder.Configuration.GetSection(MessageBrokerOptions.Name).Bind(opt));
builder.Services.AddSingleton<FibonacciValueStorage>();
builder.Services.AddSingleton((services) =>
{
    var opt = builder.Configuration.GetSection(MessageBrokerOptions.Name).Get<MessageBrokerOptions>();

    return RabbitHutch.CreateBus($"host={opt.Host};username={opt.Username};password={opt.Password}");
});
builder.Services.AddSingleton<IMessageBroker,MessageBroker>();
builder.Services.AddTransient<FibonacciCalcService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseAuthorization();

app.MapControllers();

app.Run();
