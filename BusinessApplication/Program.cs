using KafkaService.Models;
using System;
using System.Net.Http;

namespace BusinessApplication
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var orderRequest = new CreateOrderRequest();
            Console.WriteLine("Please enter User Id :");
            orderRequest.UserId = Console.ReadLine();
            Console.WriteLine("Please enter wallter Number :");
            orderRequest.WalletId = Console.ReadLine();
            Console.WriteLine("Please enter total amount");
            decimal result;
            orderRequest.TotalAmount = decimal.TryParse(Console.ReadLine(), out result) ? result : 0;
            PostOrder(orderRequest);
            Console.ReadLine();
        }


        public static void PostOrder(CreateOrderRequest createOrderRequest)
        {
            using (var client = new HttpClient())
            {
                string apiURL = "https://localhost:44384/api/order/createorder";
                var postTask = client.PostAsJsonAsync<CreateOrderRequest>(apiURL, createOrderRequest);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<bool>();
                    readTask.Wait();
                    var response = readTask.Result;
                    if(response)
                    {
                        Console.WriteLine("Order Posted successfully");
                    }
                }
            }
        }
    }
}
