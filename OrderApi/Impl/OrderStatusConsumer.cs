using Confluent.Kafka;
using KafkaService.Impl;
using KafkaService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace OrderApi.Impl
{
    public class OrderStatusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IOrderService _orderService;

        public OrderStatusConsumer(IConfiguration configuration, IHostEnvironment hostingEnvironment, IOrderService orderService)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _orderService = orderService;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bootStrapServer = AppSettings.GetConfig(_configuration, "Producer");

            var config = new ConsumerConfig
            {
                BootstrapServers = bootStrapServer?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = bootStrapServer?.Where(x => x.Key.Equals("ApiKey")).FirstOrDefault().Value,
                SaslPassword = bootStrapServer?.Where(x => x.Key.Equals("ApiSecret")).FirstOrDefault().Value,
                GroupId = "TestGroupId",
                SslCaLocation = Path.Combine(_hostingEnvironment.ContentRootPath, bootStrapServer?.Where(x => x.Key.Equals("SslCertificatePath")).FirstOrDefault().Value),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };


            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    var topicNames = new List<string> { "payment-succeded", "payment-rejected" };
                    consumerBuilder.Subscribe(topicNames);
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);
                            if (consumer != null && consumer.Topic == "payment-succeded")
                            {
                                var processPayment = JsonSerializer.Deserialize<PaymentProcessed>(consumer.Message.Value);
                                if (processPayment != null && processPayment.OrderId != 0)
                                {
                                    _orderService.OrderConfirmed(processPayment);
                                }
                            }
                            if (consumer != null && consumer.Topic == "payment-rejected")
                            {
                                var paymentRejected = JsonSerializer.Deserialize<PaymentRejected>(consumer.Message.Value);
                                if (paymentRejected != null && paymentRejected.OrderId != 0)
                                {
                                    _orderService.OrderRejected(paymentRejected);
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        consumerBuilder.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
