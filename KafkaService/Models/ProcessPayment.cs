using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaService.Models
{
    public class ProcessPayment
    {
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
