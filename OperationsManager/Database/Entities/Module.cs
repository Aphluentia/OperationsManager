using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OperationsManager.Helpers;
using System.Text.Json.Nodes;

namespace DatabaseApi.Models.Entities
{
    public class Module
    {
        [BsonId]
        public string Id { get; set; }
        public string Data { get; set; }
        public CustomModuleTemplate ModuleTemplate { get; set; }

        public DateTime Timestamp = DateTime.UtcNow;
        public string Checksum => ChecksumHelper.ComputeMD5(Data);
    }
}
