using KafkaService.Models;
using System.Threading.Tasks;

namespace OrderApi.Impl
{
    public interface IOrderService
    {
        Task<bool> CreateOrderAsync(CreateOrderRequest request);

        bool OrderConfirmed(PaymentProcessed paymentProcessed);

        bool OrderRejected(PaymentRejected paymentRejected);


    }
}