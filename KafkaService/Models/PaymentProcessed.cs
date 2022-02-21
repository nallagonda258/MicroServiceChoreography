namespace KafkaService.Models
{
    public class PaymentProcessed
    {
        public int OrderId { get; set; }

        public bool IsSuccessfull { get; set; }
    }
}
