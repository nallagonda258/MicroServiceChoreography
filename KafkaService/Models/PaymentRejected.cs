using System;
using System.Collections.Generic;
using System.Text;

namespace KafkaService.Models
{
    public class PaymentRejected
    {
        public int OrderId { get; set; }
        public string Reason { get; set; }
        public bool IsSuccessfull { get; set; }

    }
}
