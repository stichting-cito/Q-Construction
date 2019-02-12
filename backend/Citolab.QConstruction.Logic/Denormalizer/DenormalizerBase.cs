using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Citolab.QConstruction.Logic.DomainLogic.Events;
using Microsoft.Extensions.Logging;

namespace Citolab.QConstruction.Logic.Denormalizer
{
    /// <summary>
    ///     Denormalizer base
    /// </summary>
    public abstract class DenormalizerBase
    {
        private const int MaxQueueSize = 1000;

        /// <summary>
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     EventQueueHandlerTask
        /// </summary>
        protected readonly Task EventQueueHandlerTask;

        /// <summary>
        ///     EventQueue
        /// </summary>
        protected ConcurrentQueue<BaseEvent> EventQueue = new ConcurrentQueue<BaseEvent>();

        /// <summary>
        /// </summary>
        protected DateTime StartedTimestamp;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="loggerFactory"></param>
        protected DenormalizerBase(ILoggerFactory loggerFactory)
        {
            StartedTimestamp = DateTime.Now;
            _logger = loggerFactory.CreateLogger(GetType().Name);
            EventQueueHandlerTask = Task.Run(() => EventQueueHandler());
            _logger.LogInformation("Denormalizer initialized.");
        }

        private void EventQueueHandler()
        {
            while (true)
            {
                while (EventQueue.TryDequeue(out var baseEvent))
                {
                    ProcessEvent(baseEvent);
                }
                Thread.Sleep(1);
            }
            // ReSharper disable once FunctionNeverReturns
        }

        private void ProcessEvent(BaseEvent baseEvent)
        {
            var applyMethod = GetType().GetMethod("Process", new[] {baseEvent.GetType()});
            if (applyMethod == null) return;
            try
            {
                applyMethod.Invoke(this, new object[] {baseEvent});
            }
            catch (Exception exception)
            {
                _logger.LogError(null, exception, $"An error occurred while processing {baseEvent.GetType().Name}");
            }
            _logger.LogDebug($"{baseEvent.GetType().Name} dequeued.");
        }

        /// <summary>
        ///     EnqueueEvent
        /// </summary>
        /// <param name="baseEvent"></param>
        public void EnqueueEvent(BaseEvent baseEvent)
        {
            while (EventQueue.Count > MaxQueueSize)
            {
                Thread.Sleep(1);
            }
            EventQueue.Enqueue(baseEvent);
            _logger.LogDebug($"{baseEvent.GetType().Name} enqueued.");
        }
    }
}