using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Microsoft.AspNetCore.Hosting;

namespace FibonacciCalcApi.Tests
{
    internal class MockWebApplicationFactory<T> : WebApplicationFactory<T> where T : class
    {
        public Mock<IMessageBroker>? MessageBrokerMock { get; set; }
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if(MessageBrokerMock != null)
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton(MessageBrokerMock.Object);
                });
            base.ConfigureWebHost(builder);
        }
    }
}
