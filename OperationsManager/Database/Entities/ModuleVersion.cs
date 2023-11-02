using MongoDB.Bson.Serialization.Attributes;
using OperationsManager.Helpers;
using System.Text.Json;

namespace DatabaseApi.Models.Entities
{
    public class ModuleVersion
    {
        public string VersionId { get; set; }
        public string ApplicationName { get; set; }
        public string ActiveContextName { get; set; }
        public ICollection<DataPoint> DataStructure { get; set; }

        private string _htmlCard;
        public string? HtmlCard
        {
            get
            {
                return _htmlCard;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _htmlCard = $"<h3>{ApplicationName}</h3> <p>Version:{VersionId}</p><p><b>Scenario {ActiveContextName} is Active</b></p> <p><b>Last Update:<b> {Timestamp}</p>";
                }
                else
                {
                    _htmlCard = value;
                }
            }
        }
          
        public string? HtmlDashboard { get; set; }
        public DateTime Timestamp { get; set; }
        public string Checksum { get; set; }
    }
}
