using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaService.Models
{
    public class CreateOrderRequest
    {
        public string UserId { get; set; }
        public string WalletId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
