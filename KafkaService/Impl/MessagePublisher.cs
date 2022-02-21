using Confluent.Kafka;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaService.Impl
{
    public class MessagePublisher : IMessagePubisher
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;
        public MessagePublisher(IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }
        public async Task<bool> PublishAsync<T>(T message, string topicName = "")
        {
            var bootStrapServer = AppSettings.GetConfig(_configuration, "Producer");
            if (bootStrapServer != null)
            {
                var producerConfig = new ProducerConfig
                {
                    BootstrapServers = bootStrapServer?.Where(x => x.Key.Equals("BootStrapServer")).FirstOrDefault().Value,
                    SecurityProtocol = SecurityProtocol.SaslSsl,
                    SaslMechanism = SaslMechanism.Plain,
                    SaslUsername = bootStrapServer?.Where(x => x.Key.Equals("ApiKey")).FirstOrDefault().Value,
                    SaslPassword = bootStrapServer?.Where(x => x.Key.Equals("ApiSecret")).FirstOrDefault().Value,
                    SslCaLocation = Path.Combine(_hostingEnvironment.ContentRootPath, bootStrapServer?.Where(x => x.Key.Equals("SslCertificatePath")).FirstOrDefault().Value)
                };

                using (var p = new ProducerBuilder<string, string>(producerConfig).Build())
                {
                    var messg = new Message<string, string> { Key = null, Value = JsonConvert.SerializeObject(message) };
                    var topic = !string.IsNullOrWhiteSpace(topicName) ? topicName : AppSettings.GetTopicName(_configuration, "Submitted");
                    DeliveryResult<string, string> a = await p.ProduceAsync(topic, messg);
                    return a.Status == PersistenceStatus.Persisted ? true : false;
                }
            }

            return false;
        }
    }
}
