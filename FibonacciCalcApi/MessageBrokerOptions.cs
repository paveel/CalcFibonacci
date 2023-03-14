using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FibonacciCalcApp
{
    public class MessageBrokerOptions
    {
        public const string Name = nameof(MessageBrokerOptions);

        public string ExchangeName { get; set; } = "fibonacci_calc";
        public string QueueName { get; set; } = "fibonacci_queue";
        public string RoutingKey { get; set; } = "fibonacci_key";
        public string Host { get; set; } = "localhost";
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
    }
}
