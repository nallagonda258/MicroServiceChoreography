using KafkaService.Impl;
using KafkaService.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace PaymentAPI.Impl
{
    public class PaymentService : IPaymentService
    {
        private readonly IMessagePubisher _messagePubisher;

        private readonly ILogger<PaymentService> _logger;
        public PaymentService(IMessagePubisher messagePubisher, ILogger<PaymentService> logger)
        {
            _messagePubisher = messagePubisher;
            _logger = logger;
        }
        public async Task<bool> ProcessPayment(ProcessPayment payment)
        {
            _logger.LogInformation("Consumed order-created topic with Payload " + JsonConvert.SerializeObject(payment));

            if (payment.OrderId!=0 && payment.TotalAmount > 500)
            {
                var paymentProcessed = new PaymentProcessed { OrderId = payment.OrderId, IsSuccessfull = true };
                var result =await _messagePubisher.PublishAsync<PaymentProcessed>(paymentProcessed, "payment-succeded");
                _logger.LogInformation("produced payment-succeded topic with Payload " + JsonConvert.SerializeObject(paymentProcessed));
                return result;
            }
            else
            {
                var paymentRejected = new PaymentRejected { OrderId = payment.OrderId, Reason = "testing Reason", IsSuccessfull = false };
                var result = await _messagePubisher.PublishAsync<PaymentRejected>(paymentRejected, "payment-rejected");
                _logger.LogInformation("produced payment-rejected topic with Payload " + JsonConvert.SerializeObject(paymentRejected));
                return result;
            }
        }
    }
}
