using System.Collections.Concurrent;

namespace FibonacciCalc
{
    /// <summary>
    /// Хранит все последние рассчитанные значения последовательности фибоначчи для каждого расчета
    /// </summary>
    public class FibonacciValueStorage
    {
        private readonly ConcurrentDictionary<int, long> _steps = new();

        /// <summary>
        /// Рассчитать следующее значение последовательности фибоначчи
        /// </summary>
        /// <param name="calcValue">текущее значение</param>
        /// <returns>новое значение</returns>
        /// <exception cref="ArgumentException"></exception>
        public FibonacciValue CalcNextAndUpate(FibonacciValue calcValue)
            => new()
            {
                    CalcId = calcValue.CalcId,
                    Value = _steps.AddOrUpdate(calcValue.CalcId, (key) =>
                    {
                        if (calcValue.Value != 1)
                            throw new ArgumentException($"For first step {nameof(calcValue.Value)} value must be equals 1");

                        return 1;
                    },
                (key, oldValue) =>
                {
                    if (calcValue.Value != 1 && calcValue.Value <= oldValue)
                        throw new ArgumentException($"{nameof(calcValue.Value)} can't be equal or less" +
                            $"old value except if old value equals 1");

                    return oldValue + calcValue.Value;
                })
            };           
    }
}