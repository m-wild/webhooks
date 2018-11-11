using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Producer.Entities;
using Producer.Repositories;

namespace Producer.Infrastructure
{
    public interface IEventQueueWorker
    {
        Task ProcessEvent(Event evnt);
    }
    
    public class EventQueueWorker : IEventQueueWorker
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IEventRepository _eventRepository;
        private readonly ILogger _logger;
        private readonly HttpClient _httpClient;

        public EventQueueWorker(ISubscriptionRepository subscriptionRepository, IEventRepository eventRepository, HttpClient httpClient, ILoggerFactory loggerFactory)
        {
            _subscriptionRepository = subscriptionRepository;
            _eventRepository = eventRepository;
            _logger = loggerFactory.CreateLogger<EventQueueWorker>();
            _httpClient = httpClient;
        }


        public async Task ProcessEvent(Event evnt)
        {
            var subscriptions = _subscriptionRepository.GetAll();

            var tasks = new List<Task>();
            foreach (var sub in subscriptions)
            {
                tasks.Add(SendWebhook(sub, evnt));
            }
            
            await Task.WhenAll(tasks);
        }

        private async Task SendWebhook(Subscription sub, Event evnt)
        {
            try
            {
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, sub.Uri);
                httpRequest.Content = new StringContent(evnt.Body, Encoding.UTF8, "application/json");
    
                var response = await _httpClient.SendAsync(httpRequest);

                response.EnsureSuccessStatusCode();
                
                evnt.PublishedAt = DateTime.Now;
                _eventRepository.UpdateEvent(evnt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending event {evnt.EventId} to subscription {sub.Name}");
            }
        }
    }
}