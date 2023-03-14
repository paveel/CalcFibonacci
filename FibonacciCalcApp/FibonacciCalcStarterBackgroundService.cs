using EasyNetQ;
using FibonacciCalc;
using FibonacciCalc.ApiClient;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using CodeJam;

namespace FibonacciCalcApp
{
    /// <summary>
    /// Инициализация первоначального расчета
    /// </summary>
    internal class FibonacciCalcStarterBackgroundService : BackgroundService
    {
        private readonly FibonacciValueStorage _fibonacciValueStorage;
        private readonly FibonacciCalcClient _fibonacciCalcClient; 
        private readonly FibonacciCalcOptions _fibonacciCalcOptions;
        private readonly ILogger<FibonacciCalcStarterBackgroundService> _logger;

        public FibonacciCalcStarterBackgroundService(
            FibonacciValueStorage fibonacciValueStorage,
            FibonacciCalcClient fibonacciCalcClient,
            IOptions<FibonacciCalcOptions> fibonacciCalcOptions,
            ILogger<FibonacciCalcStarterBackgroundService> logger)
        {
            Code.NotNull(fibonacciValueStorage, nameof(fibonacciValueStorage));
            Code.NotNull(fibonacciCalcClient, nameof(fibonacciCalcClient));
            Code.NotNull(fibonacciCalcOptions, nameof(fibonacciCalcOptions));
            Code.NotNull(logger, nameof(logger));

            _fibonacciValueStorage = fibonacciValueStorage;
            _fibonacciCalcClient = fibonacciCalcClient;
            _fibonacciCalcOptions = fibonacciCalcOptions.Value;
            _logger = logger;
        }

        /// <summary>
        /// Рассчитывает первое значение и отправляет по api в другой сервис
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"Start initialize fibonacci calcs for {_fibonacciCalcOptions.CalcTaskCount} tasks");

            var tasks = new List<Task>();

            var counter = new Counter();

            for (var i = 0; i < _fibonacciCalcOptions.CalcTaskCount; i++)
            {
                tasks.Add(Task.Run(async () =>
                {
                    var calcId = -1;
                    try
                    {
                        calcId = counter.NextValue();
                        _logger.LogInformation($"start new task for calcId={calcId}");

                        var nextValue = _fibonacciValueStorage.CalcNextAndUpate(
                        new FibonacciValue
                        {
                            CalcId = calcId,
                            Value = 1
                        });

                        await _fibonacciCalcClient.CalcStep(nextValue, stoppingToken);

                        _logger.LogInformation($"Successfully started new task for calcId={calcId}");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error while initialize calc for calcId={calcId}");
                    }
                }, stoppingToken));
            }

            await Task.WhenAll(tasks);
        }

        public sealed class Counter
        {
            private int current = -1;
            public int NextValue()
            {
                return Interlocked.Increment(ref current);
            }
        }
    }
}
