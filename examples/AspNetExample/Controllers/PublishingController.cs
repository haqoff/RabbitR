using Microsoft.AspNetCore.Mvc;
using RabbitR.Producers;

namespace AspNetExample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PublishingController : ControllerBase
    {
        private readonly IEventBusPublisher _publisher;

        public PublishingController(IEventBusPublisher publisher)
        {
            _publisher = publisher;
        }

        [HttpPost(Name = "Publish")]
        public void Publish([FromBody] CreateCustomerRequestMessage m)
        {
            _publisher.PublishToExchangeAsync("create_customer_request_exchange", m);
        }
    }
}