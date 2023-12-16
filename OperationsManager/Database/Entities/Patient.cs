using OperationsManager.Database.Entities;

namespace DatabaseApi.Models.Entities
{
    public class Patient: User
    {
        public string ConditionName { get; set; }
        public DateTime ConditionAcquisitionDate { get; set; }
        public ICollection<Module>? Modules { get; set; }
        public HashSet<string>? AcceptedTherapists { get; set; } = null;
        public HashSet<string>? RequestedTherapists { get; set; } = null;

    }
}
