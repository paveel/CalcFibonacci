namespace FibonacciCalc.Tests
{
    public class FibonacciValueStorageTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CalcNextAndUpate_CallToCalcSequence_CorrectFibonacciSequence()
        {
            var valueStorage1 = new FibonacciValueStorage();
            var valueStorage2 = new FibonacciValueStorage();

            var fibonacciExpectedValues = new int[] { 1, 1, 2, 3, 5, 8, 13, 21, 34, 55 };

            var values = new List<long>();
            var currentValue = new FibonacciValue { CalcId = 0, Value = 1 };

            for (var i = 0; i < 5; i++)
            {
                currentValue = valueStorage1.CalcNextAndUpate(currentValue);
                values.Add(currentValue.Value);
                currentValue = valueStorage2.CalcNextAndUpate(currentValue);
                values.Add(currentValue.Value);
            }

            Assert.Multiple(() =>
            {
                for (var i = 0; i < fibonacciExpectedValues.Length; i++)
                {
                    Assert.That(fibonacciExpectedValues[i] == values[i],
                            $"Incorrect fibonacci sequence value: {currentValue}, " +
                            $"expected: {fibonacciExpectedValues[i]}");
                }
            });
        }

        [Test]
        public void CalcNextAndUpate_CallIncorrectValue_ThrowException()
        {
            var valueStorage = new FibonacciValueStorage();

            var next = valueStorage.CalcNextAndUpate(new FibonacciValue { CalcId = 0, Value = 1 });
            valueStorage.CalcNextAndUpate(next);

            Assert.Throws<ArgumentException>(() =>
                valueStorage.CalcNextAndUpate(new FibonacciValue { CalcId = 0, Value = -10 }));
        }
    }
}