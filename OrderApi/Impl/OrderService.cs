using KafkaService.Impl;
using KafkaService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace OrderApi.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IMessagePubisher _messagePubisher;

        private readonly ILogger<OrderService> _logger;

        public OrderService(IMessagePubisher messagePubisher, ILogger<OrderService> logger)
        {
            _messagePubisher = messagePubisher;
            _logger = logger;
        }
        public async Task<bool> CreateOrderAsync(CreateOrderRequest request)
        {
            Random random = new Random();
            // Order creation logic in "Pending" state
            var processPayment = new ProcessPayment { OrderId = random.Next(), TotalAmount = request.TotalAmount };
            await _messagePubisher.PublishAsync<ProcessPayment>(processPayment);
            
            _logger.LogInformation("Produced order-created topic with Payload " + JsonConvert.SerializeObject(processPayment));
            return true;
        }

        public bool OrderConfirmed(PaymentProcessed paymentProcessed)
        {
            //database code to confirm order goes here.
            _logger.LogInformation("Consumed payment-succeeded topic with Payload " + JsonConvert.SerializeObject(paymentProcessed));

            return true;
        }

        public bool OrderRejected (PaymentRejected paymentRejected)
        {
            // database code to reject order goes here.
            _logger.LogInformation("Consumed payment-rejected topic with Payload " + JsonConvert.SerializeObject(paymentRejected));

            return false;
        }
    }
}
