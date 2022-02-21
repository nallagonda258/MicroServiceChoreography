using KafkaService.Models;
using System.Threading.Tasks;

namespace PaymentAPI.Impl
{
    public interface IPaymentService
    {
        Task<bool> ProcessPayment(ProcessPayment payment);
    }
}
