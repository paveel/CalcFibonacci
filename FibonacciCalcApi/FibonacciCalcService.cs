using FibonacciCalc;
using CodeJam;

namespace FibonacciCalcApi
{
    /// <summary>
    /// Класс содержить логику для расчета нового значения и отправки через брокер сообщений
    /// </summary>
    public class FibonacciCalcService
    {
        private readonly IMessageBroker _messageBroker;
        private readonly FibonacciValueStorage _fibonacciValueStorage;
        private readonly ILogger<FibonacciCalcService> _logger;
        public FibonacciCalcService(
            IMessageBroker messageBroker,
            FibonacciValueStorage fibonacciValueStorage,
            ILogger<FibonacciCalcService> logger)
        {
            Code.NotNull(messageBroker, nameof(messageBroker));
            Code.NotNull(fibonacciValueStorage, nameof(fibonacciValueStorage));
            Code.NotNull(logger, nameof(logger));

            _messageBroker = messageBroker;
            _fibonacciValueStorage = fibonacciValueStorage;
            _logger = logger;
        }

        public async Task CalcStepAsync(FibonacciValue fibonacciValue, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Received fibonacciValue calcId={fibonacciValue.CalcId}, value={fibonacciValue.Value}");
            var nextValue = _fibonacciValueStorage.CalcNextAndUpate(fibonacciValue);
            await _messageBroker.PublishAsync(nextValue, cancellationToken);
        }
    }
}
