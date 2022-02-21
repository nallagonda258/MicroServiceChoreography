using Confluent.Kafka;
using KafkaService.Impl;
using KafkaService.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentAPI.Impl
{
    public class PaymentConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;
        private readonly IHostingEnvironment _hostingEnvironment;


        public PaymentConsumer(IConfiguration configuration, IPaymentService paymentService, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _hostingEnvironment = hostingEnvironment;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            var bootStrapServer = AppSettings.GetConfig(_configuration, "Consumer");

            var config = new ConsumerConfig
            {
                BootstrapServers = bootStrapServer?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                SaslMechanism = SaslMechanism.Plain,
                SaslUsername = bootStrapServer?.Where(x => x.Key.Equals("ApiKey")).FirstOrDefault().Value,
                SaslPassword = bootStrapServer?.Where(x => x.Key.Equals("ApiSecret")).FirstOrDefault().Value,
                GroupId ="TestGroupId",
                SslCaLocation = Path.Combine(_hostingEnvironment.ContentRootPath, bootStrapServer?.Where(x => x.Key.Equals("SslCertificatePath")).FirstOrDefault().Value),
                AutoOffsetReset = AutoOffsetReset.Earliest
            };


            try
            {
                using (var consumerBuilder = new ConsumerBuilder<Ignore, string>(config).Build())
                {
                    consumerBuilder.Subscribe("order-created");
                    var cancelToken = new CancellationTokenSource();

                    try
                    {
                        while (true)
                        {
                            var consumer = consumerBuilder.Consume(cancelToken.Token);
                            var processPayment = JsonSerializer.Deserialize<ProcessPayment>(consumer.Message.Value);
                            if (processPayment != null)
                            {
                                _paymentService.ProcessPayment(processPayment);
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
            return Task.CompletedTask;
        }
    }
}

