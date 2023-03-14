using Newtonsoft.Json;
using System.Net.Mime;
using System.Text;

namespace FibonacciCalc.ApiClient
{
    /// <summary>
    /// Класс-клиент для веб-сервиса
    /// </summary>
    public class FibonacciCalcClient
    {
        private readonly HttpClient _httpClient;
        public FibonacciCalcClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task CalcStep(FibonacciValue currentValue, CancellationToken cancellationToken = default)
        {
            var requestUri = $"api/FibonacciCalculator?calcId={currentValue.CalcId}&value={currentValue.Value}";

            using var response = await _httpClient.GetAsync(requestUri, cancellationToken);
            using var respMessage = response.EnsureSuccessStatusCode();
        }
    }
}