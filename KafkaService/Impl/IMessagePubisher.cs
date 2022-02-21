using System.Threading.Tasks;

namespace KafkaService.Impl
{
    public interface IMessagePubisher
    {
        /// <summary>
        /// produces message to given topic
        /// </summary>
        /// <param name="request">request</param>
        /// <returns></returns>
        Task<bool> PublishAsync<T>(T request, string topicName="");
    }
}