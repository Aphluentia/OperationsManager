using MongoDB.Bson.Serialization.Attributes;
using OperationsManager.Helpers;
using System.Text.Json;

namespace DatabaseApi.Models.Entities
{
    public class ModuleVersion
    {
        public string VersionId { get; set; }
        public string ApplicationName { get; set; }
        public ICollection<DataPoint> DataStructure { get; set; }
        public string HtmlCard => $"<h3>{ApplicationName}</h3> <p>Version:{VersionId}</p> <p><b>Last Update:<b> {Timestamp}</p>";
        public string? HtmlDashboard { get; set; }
        public DateTime Timestamp { get; set; }
        public string Checksum => ChecksumHelper.ComputeMD5(JsonSerializer.Serialize(DataStructure));
    }
}
