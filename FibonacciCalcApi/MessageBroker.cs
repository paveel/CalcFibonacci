using EasyNetQ;
using EasyNetQ.Topology;
using FibonacciCalc;
using FibonacciCalcApp;
using CodeJam;

namespace FibonacciCalcApi
{
    /// <summary>
    /// Класс который отвечает за инициализацию брокера сообщений и публикацию через нужный exchange
    /// </summary>
    public class MessageBroker : IMessageBroker
    {
        private readonly IBus _bus;
        private readonly MessageBrokerOptions _messageBrokerOptions;
        private Exchange _exchange;
        private ILogger<MessageBroker> _logger;
        private bool _isInitialized;

        public MessageBroker(
            IBus bus,
            MessageBrokerOptions messageBrokerOptions,
            ILogger<MessageBroker> logger)
        {
            Code.NotNull(messageBrokerOptions, nameof(messageBrokerOptions));
            Code.NotNull(logger, nameof(logger));

            _bus = bus;
            _messageBrokerOptions = messageBrokerOptions;
            _logger = logger;
        }

        /// <summary>
        /// Инициализировать rabbitmq для обмена сообщениями - создать exchange, qeueu и сделать bind
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task Init(CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Start Initialize message broker");

                _exchange = await _bus.Advanced.ExchangeDeclareAsync(
                    _messageBrokerOptions.ExchangeName,
                    "direct",
                    true,
                    false,
                cancellationToken);

                var queue = await _bus.Advanced.QueueDeclareAsync(_messageBrokerOptions.QueueName, cancellationToken);
                await _bus.Advanced.BindAsync(_exchange, queue, _messageBrokerOptions.RoutingKey, cancellationToken);

                _logger.LogInformation($"Was created and binded exchange={_exchange.Name}, queue={queue.Name}");

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while initializing message broker");

                throw;
            }
        }

        public async Task PublishAsync(FibonacciValue value, CancellationToken cancellationToken)
        {
            if (!_isInitialized)
                await Init(cancellationToken);

            try
            {
                await _bus.Advanced.PublishAsync(
                    _exchange,
                    _messageBrokerOptions.RoutingKey,
                    false,
                    new Message<FibonacciValue>(value),
                    cancellationToken);

                _logger.LogInformation($"Successfully published message with calcId={value.CalcId}, value={value.Value}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while publishing message to exchange={_exchange.Name}, " +
                    $"  routingKey={_messageBrokerOptions.RoutingKey}," +
                    $" calcId={value.CalcId}, value={value.Value}");

                throw;
            }
        }
    }
}