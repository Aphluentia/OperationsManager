using DatabaseApi.Models.Entities;
using OperationsManager.Database.Entities;
using OperationsManager.Models.BrokerMessageDataField;

namespace OperationsManager.Database
{
    public interface IDatabaseProvider
    {
        // Application
        Task<ActionResponse> RegisterApplication(Application data);
        Task<ActionResponse> UpdateApplicationVersion(string ApplicationId, string VersionId, ModuleVersion data);
        Task<ActionResponse> RegisterApplicationVersion(string id, ModuleVersion data);
        Task<ActionResponse> DeleteApplication(string id);
        Task<ActionResponse> DeleteApplicationVersion(string id, string VersionId);

        // Modules
        Task<ActionResponse> CreateModule(Module module);
        Task<ActionResponse> DeleteModule(string id);
        Task<ActionResponse> UpdateModule(string id, ModuleVersion data);
        Task<ActionResponse> UpdateModuleToVersion(string id,string versionId);

        // Patients
        Task<ActionResponse> CreatePatient(Patient patient);
        Task<ActionResponse> UpdatePatient(string Email, Patient data);
        Task<ActionResponse> UpdatePatientModule(string Email, Guid ModuleId, Module data);
        Task<ActionResponse> UpdatePatientModuleToVersion(string Email, Guid ModuleId, string VersionId);
        Task<ActionResponse> AddExistingModuleToPatient(string Email, string id);
        Task<ActionResponse> AddModuleToPatient(string Email, Module data);
        Task<ActionResponse> PatientRejectTherapist(string Email, string Therapist);
        Task<ActionResponse> PatientRequestTherapist(string Email, string Therapist);
        Task<ActionResponse> DeletePatient(string Email);
        Task<ActionResponse> DeletePatientModule(string id, Guid ModuleId);

        // Therapist
        Task<ActionResponse> CreateTherapist(Therapist therapist);
        Task<ActionResponse> UpdateTherapist(string Email, Therapist data);
        Task<ActionResponse> TherapistAcceptPatient(string Email, string Patient);
        Task<ActionResponse> TherapistRejectPatient(string Email, string Patient);
        Task<ActionResponse> DeleteTherapist(string Email);




    }
}