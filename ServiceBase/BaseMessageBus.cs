using Serilog;
using System;

using EasyNetQ;
using Newtonsoft.Json;
using JsonSerializer = EasyNetQ.JsonSerializer;

namespace ServiceBase
{
    /// <summary>
    /// Реализует очередь сообщений для обмена между сервисами.
    /// </summary>
    public class BaseMessageBus
    {
        private readonly IBus _bus;
        private readonly string _connectionString;

        public BaseMessageBus(string connectionString)
        {
            _connectionString = connectionString;

            // Отключаем проверку типов, потому что в свойство базового класса можем записать данные 
            // из типа производного, которого нет в сборке на принимающей стороне.
            var settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.None
            };

            _bus = RabbitHutch.CreateBus(_connectionString, 
                opts => opts.Register<ISerializer>(_ => new JsonSerializer(settings)));
        }

        /// <summary>
        /// Публикует сообщение с типом T
        /// </summary>
        public void Publish<T>(T message) where T : class
        {
            _bus.PubSub.Publish(message);
        }

        /// <summary>
        /// Производит подписку на сообщение с типом T
        /// </summary>
        /// <param name="subscriptionId">Идентификатор для конкурентной обработки сообщений</param>
        public void Subscribe<T>(Action<T> onMessage, string subscriptionId = null) where T : class
        {
            bool ok = false;
            while (!ok)
            {
                ok = TrySubscribe(onMessage, subscriptionId);
            }
        }

        private bool TrySubscribe<T>(Action<T> onMessage, string subscriptionId) where T : class
        {
            try
            {
                var id = subscriptionId ?? Guid.NewGuid().ToString();
                _bus.PubSub.Subscribe(id, onMessage, config => config.WithAutoDelete());
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Rabbit MQ subscribe error.");
                return false;
            }
        }
    }
}
