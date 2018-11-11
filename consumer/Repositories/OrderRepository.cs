using System.Net.Http;

namespace Consumer.Repositories
{
    public interface IOrderRepository
    {
        void AcknowledgeEvent(int eventId);
    }
    
    public class OrderRepository : IOrderRepository
    {
        private readonly HttpClient _httpClient;

        public OrderRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        
        
        public void AcknowledgeEvent(int eventId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"http://localhost:5000/api/events/{eventId}/acknowledge");

            var response = _httpClient.SendAsync(request).GetAwaiter().GetResult();

            response.EnsureSuccessStatusCode();
        }
    }
}