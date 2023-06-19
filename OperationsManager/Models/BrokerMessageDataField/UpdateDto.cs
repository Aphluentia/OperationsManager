namespace OperationsManager.Models.BrokerMessageDataField
{
    public class UpdateDto<T>
    {
        public string Id { get; set; }
        public string? Id2 { get; set; }
        public string? Id3 { get; set; }
        public T Data { get; set; }
    }
}
