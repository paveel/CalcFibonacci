using FibonacciCalc.ApiClient;
using FibonacciCalc;
using Moq;
using AutoFixture;

namespace FibonacciCalcApi.Tests
{
    public class Tests
    {
        private MockWebApplicationFactory<Program> _webAppFactory;
        [OneTimeSetUp]
        public void OnceSetup()
        {
            _webAppFactory = new MockWebApplicationFactory<Program>();
        }

        [SetUp]
        public void Setup()
        {
            var fixture = new Fixture();
            var messageBrokerMock = fixture.Freeze<Mock<IMessageBroker>>();
            messageBrokerMock.Setup(x => x.PublishAsync(It.IsAny<FibonacciValue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _webAppFactory.MessageBrokerMock = messageBrokerMock;
        }

        [Test]
        public async Task CalcStep_CorrectValue_CAlledPublishAsync()
        {
            var value = new FibonacciValue { CalcId = 0, Value = 1 };
   
            using var httpClient = _webAppFactory.CreateClient();
            var apiClient = new FibonacciCalcClient(httpClient);


            await apiClient.CalcStep(value);

            _webAppFactory.MessageBrokerMock.Verify(mb => mb.PublishAsync(It.IsAny<FibonacciValue>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}