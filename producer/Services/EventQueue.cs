using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Producer.Entities;
using Producer.Repositories;
using Timer = System.Timers.Timer;

namespace Producer.Services
{
    public interface IEventQueue
    {
        void Add(EventSubscription eventSubscription);
    }

    public class EventQueue : IEventQueue, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly BlockingCollection<EventSubscription> _queue;
        private readonly Task _dequeueTask;
        private readonly Timer _timer;
        
        public EventQueue(HttpClient httpClient, ILoggerFactory loggerFactory, IServiceProvider serviceProvider)
        {

            _httpClient = httpClient;
            
            // this class has to be singleton as we run other threads and timers here... each thread has to create a new
            // scope to resolve scoped services.
            _serviceProvider = serviceProvider;
            _logger = loggerFactory.CreateLogger<EventService>();

            _queue = new BlockingCollection<EventSubscription>();

            _dequeueTask = Task.Run(DequeueTask, _cancellationTokenSource.Token);
            
            _timer = new Timer
            {
                Interval = TimeSpan.FromSeconds(5).TotalMilliseconds,
                AutoReset = false,
            };
            _timer.Elapsed += AddUnpublishedTasks;
            _timer.Start();
        }
        
        public void Add(EventSubscription eventSubscription)
        {
            _queue.Add(eventSubscription);
        }
        
        
        private async Task DequeueTask()
        {
            while (!_queue.IsCompleted)
            {
                try // we must never exit this loop!
                {
                    var eventSubscription = _queue.Take(_cancellationTokenSource.Token);
                    await PublishEventSubscription(eventSubscription);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled exception while processing event");
                }
            }
        }
        
        
        private async Task PublishEventSubscription(EventSubscription eventSubscription)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();
                var eventRepository = scope.ServiceProvider.GetRequiredService<IEventRepository>();
                var db = scope.ServiceProvider.GetRequiredService<IDatabase>();
                
                // get details
                var sub = subscriptionRepository.GetById(eventSubscription.SubscriptionId);
                var evnt = eventRepository.GetById(eventSubscription.EventId);

                // send the webhook
                try
                {
                    var httpRequest = new HttpRequestMessage(HttpMethod.Post, sub.Uri);
                    httpRequest.Content = new StringContent(JsonConvert.SerializeObject(evnt), Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(httpRequest);

                    response.EnsureSuccessStatusCode();

                    // mark as complete
                    db.BeginTransaction();
                    
                    eventSubscription.PublishedAt = DateTime.Now;
                    subscriptionRepository.UpdateEventSubscription(eventSubscription);
                    
                    db.Commit();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error sending event {evnt.EventId} to subscription {sub.Name}");
                }
            }
        }


        private void AddUnpublishedTasks(object s, EventArgs args)
        {
            // takes care of events that failed to publish...
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var subscriptionRepository = scope.ServiceProvider.GetRequiredService<ISubscriptionRepository>();

                    var unpublished = subscriptionRepository.GetUnpublishedEventSubscriptions()
                        // created well before the previous run
                        .Where(es => es.CreatedAt < DateTime.Now.AddMilliseconds(1 - (_timer.Interval * 2)));

                    foreach (var es in unpublished)
                    {
                        _queue.Add(es);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception while re-queuing events");
            }
            finally
            {
                _timer.Start();
            }
        }
        

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _dequeueTask?.Dispose();
        }
    }
}