using EasyNetQ;
using FibonacciCalc;
using FibonacciCalc.ApiClient;
using FibonacciCalcApp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using System.Runtime;

var hostBuilder = Host.CreateDefaultBuilder(args);

var host = hostBuilder.ConfigureHostConfiguration(builder =>
    {
        builder.SetBasePath(Directory.GetCurrentDirectory());
        builder.AddCommandLine(args);
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
        builder.AddJsonFile("appsettings.json", optional: true);
        builder.AddJsonFile(
            $"appsettings.{context.HostingEnvironment.EnvironmentName}.json",
            optional: true);
        builder.AddCommandLine(args);
        builder.Build();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging(builder =>
        {
            var logger = new LoggerConfiguration()
                         .ReadFrom.Configuration(context.Configuration)
                          .Enrich.FromLogContext()
                          .CreateLogger();
            builder.AddSerilog(logger);
        })
        .AddSingleton<Serilog.ILogger>(sp =>
        {
            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .CreateLogger();
        })
        .BuildServiceProvider();

        services.AddHostedService<FibonacciCalcStarterBackgroundService>()
                .AddHostedService<FibonacciCalcBackgroundService>()
                .AddSingleton<FibonacciValueStorage>()
                .AddSingleton((services) => {
                    var opt = context.Configuration.GetSection(MessageBrokerOptions.Name).Get<MessageBrokerOptions>();

                        return RabbitHutch.CreateBus($"host={opt.Host};username={opt.Username};password={opt.Password}");
                    })
                .Configure<FibonacciCalcOptions>(opt =>
                    context.Configuration.GetSection(FibonacciCalcOptions.Name).Bind(opt))
                .Configure<MessageBrokerOptions>(opt =>
                    context.Configuration.GetSection(MessageBrokerOptions.Name).Bind(opt));

        services.AddHttpClient<FibonacciCalcClient>((client) =>
        {
            client.BaseAddress = new Uri(context.Configuration.GetSection(FibonacciApiOptions.Name)
                                                              .Get<FibonacciApiOptions>().Url);
        });
    })
    .Build();

host.Run();