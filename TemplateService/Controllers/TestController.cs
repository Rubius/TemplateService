using ServiceBase;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TemplateService.Controllers
{
    class TestMessage
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// Тестовый контроллер.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly BaseMessageBus _messageBus;

        public TestController(BaseMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        /// <summary>
        /// Метод для подписки на сообщение в RabbitMQ.
        /// </summary>
        [HttpGet("subscribe")]
        public ActionResult<string> Subscribe()
        {
            var id = Guid.NewGuid();
            _messageBus.Subscribe<TestMessage>(m => Console.WriteLine($"Subscribe id: {id} received string: {m.Message}"));
            
            return Ok();
        }

        /// <summary>
        /// Метод для публикации сообщения в RabbitMQ.
        /// </summary>
        [HttpGet("publish")]
        public ActionResult<string> Publish(string text)
        {
            _messageBus.Publish(new TestMessage { Message = text});
            return Ok(text);
        }
    }
}