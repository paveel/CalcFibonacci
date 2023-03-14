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
    /// Обработчик сообщений от rabbirmq для расчета следующего значения последовательности
    /// </summary>
    internal class FibonacciCalcBackgroundService : BackgroundService
    {
        private readonly IBus _bus;
        private readonly FibonacciValueStorage _fibonacciValueStorage;
        private readonly FibonacciCalcClient _fibonacciCalcClient;
        private readonly MessageBrokerOptions _messageBrokerOptions;
        private readonly ILogger<FibonacciCalcBackgroundService> _logger;

        public FibonacciCalcBackgroundService(
            IBus bus,
            FibonacciValueStorage fibonacciValueStorage,
            FibonacciCalcClient fibonacciCalcClient,
            IOptions<MessageBrokerOptions> messageBrokerOptions,
            ILogger<FibonacciCalcBackgroundService> logger)
        {
            Code.NotNull(fibonacciValueStorage, nameof(fibonacciValueStorage));
            Code.NotNull(fibonacciCalcClient, nameof(fibonacciCalcClient));
            Code.NotNull(messageBrokerOptions, nameof(messageBrokerOptions));
            Code.NotNull(logger, nameof(logger));

            _bus = bus;
            _fibonacciValueStorage = fibonacciValueStorage;
            _fibonacciCalcClient = fibonacciCalcClient;
            _messageBrokerOptions = messageBrokerOptions.Value;
            _logger = logger;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var existingQueue = new EasyNetQ.Topology.Queue(_messageBrokerOptions.QueueName);

            _bus.Advanced.Consume<FibonacciValue>(existingQueue, HandleMessageAsync);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Обработать сообщений из rabbitmq, рассчитать новое значение и отправить через api
        /// </summary>
        /// <param name="message"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        private async Task HandleMessageAsync(IMessage<FibonacciValue> message, MessageReceivedInfo info)
        {
            try
            {
                _logger.LogInformation($"Received message exchange={info.Exchange}, queue={info.Exchange}," +
                    $"value={message.Body.Value}, calcId={message.Body.CalcId}");

                var nextValue = _fibonacciValueStorage.CalcNextAndUpate(new FibonacciValue
                {
                    CalcId = message.Body.CalcId,
                    Value = message.Body.Value
                });

                _logger.LogInformation($"New value={nextValue.Value} for calcId={nextValue.CalcId}");

                await _fibonacciCalcClient.CalcStep(nextValue);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while handling message");
            }
        }
    }
}
