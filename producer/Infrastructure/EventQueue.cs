using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Producer.Entities;
using Producer.Repositories;

namespace Producer.Infrastructure
{
    public interface IEventQueue
    {
        void Add(Event evnt);
    }
    
    public class EventQueue : IEventQueue, IDisposable
    {
        private readonly IEventQueueWorker _worker;
        private readonly IEventRepository _eventRepository;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly BlockingCollection<Event> _queue;
        private readonly Task _dequeueTask;
        
        public EventQueue(IEventQueueWorker worker, IEventRepository eventRepository)
        {
            _worker = worker;
            _eventRepository = eventRepository;
            
            _queue = new BlockingCollection<Event>();

            _dequeueTask = Task.Run(ProcessEvents, _cancellationTokenSource.Token);
        }
        
        public void Add(Event evnt)
        {
            _eventRepository.CreateEvent(evnt);
            _queue.Add(evnt);
        }

        private async Task ProcessEvents()
        {
            while (!_queue.IsCompleted)
            {
                try // we must never exit this loop!
                {
                    var evnt = _queue.Take(_cancellationTokenSource.Token);
                    await _worker.ProcessEvent(evnt);
                }
                catch (Exception)
                {
                    // log
                }
            }

        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _dequeueTask?.Dispose();
        }

    }
}