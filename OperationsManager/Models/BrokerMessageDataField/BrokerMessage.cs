using OperationsManager.Models.Enums;

namespace OperationsManager.Models.BrokerMessageDataField
{
    public class BrokerMessage
    {
        public Guid TaskId { get; set; }
        public Operation OperationCode { get; set; }
        public object Data { get; set; }
    }
}
