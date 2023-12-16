using OperationsManager.Models.BrokerMessageDataField;

namespace OperationsManager.BackgroundServices.Workers
{
    public interface IKafkaMessageHandler
    {
        public BrokerMessage FetchIncomingMessage();
    }
}
