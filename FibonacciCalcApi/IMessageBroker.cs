using FibonacciCalc;

namespace FibonacciCalcApi
{
    public interface IMessageBroker
    {
        Task PublishAsync(FibonacciValue value, CancellationToken cancellationToken);
    }
}