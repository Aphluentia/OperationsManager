using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using OperationsManager.Helpers;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace DatabaseApi.Models.Entities
{
    public class Module
    {
        public Guid Id { get; set; }
        public ModuleVersion ModuleData { get; set; }

    }
}
