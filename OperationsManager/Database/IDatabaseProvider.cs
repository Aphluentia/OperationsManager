using DatabaseApi.Models.Entities;
using OperationsManager.Models.BrokerMessageDataField;

namespace OperationsManager.Database
{
    public interface IDatabaseProvider
    {
        Task<bool> AddExistingModuleToPatient(string Email, string id);
        Task<bool> AddModuleToPatient(string Email, Module data);
        Task<bool> CreateModule(Module module);
        Task<bool> CreatePatient(Patient patient);
        Task<bool> CreateTherapist(Therapist therapist);
        Task<bool> DeleteApplication(string id);
        Task<bool> DeleteApplicationVersion(string id, string VersionId);
        Task<bool> DeleteModule(string id);
        Task<bool> DeletePatient(string Email);
        Task<bool> DeletePatientModule(string id, string ModuleId);
        Task<bool> DeleteTherapist(string Email);
        Task<bool> PatientRejectTherapist(string Email, string Therapist);
        Task<bool> PatientRequestTherapist(string Email, string Therapist);
        Task<bool> RegisterApplication(Application data);
        Task<bool> RegisterApplicationVersion(string id, ModuleVersion data);
        Task<bool> TherapistAcceptPatient(string Email, string Patient);
        Task<bool> TherapistRejectPatient(string Email, string Patient);
        Task<bool> UpdateApplicationVersion(string ApplicationId, string VersionId, ModuleVersion data);
        Task<bool> UpdateModule(string id, Module data);
        Task<bool> UpdateModuleToVersion(string id, string VersionId);
        Task<bool> UpdateModuleVersion(string id, Module data);
        Task<bool> UpdatePatient(string Email, Patient data);
        Task<bool> UpdatePatientModule(string Email, string ModuleId, Module data);
        Task<bool> UpdatePatientModuleToVersion(string Email, string ModuleId, string VersionId);
        Task<bool> UpdateTherapist(string Email, Therapist data);
    }
}