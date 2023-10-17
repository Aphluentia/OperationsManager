using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using DatabaseApi.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using Newtonsoft.Json;
using OperationsManager.Configurations;
using OperationsManager.Database.Entities;
using OperationsManager.Helpers;

namespace OperationsManager.Database
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private readonly string _Therapists, _Modules, _Applications, _Patients;
        public DatabaseProvider(IOptions<DatabaseApiConfigSection> options)
        {
            _Therapists = $"{options.Value.ConnectionString}/api/Therapist";
            _Modules = $"{options.Value.ConnectionString}/api/Modules";
            _Applications = $"{options.Value.ConnectionString}/api/Application";
            _Patients = $"{options.Value.ConnectionString}/api/Patient";

        }
        // Application ----------------------------------------------------------------------------------
        public async Task<ActionResponse> RegisterApplication(Application data)
        {
            var url = $"{_Applications}";

            foreach (ModuleVersion v in data.Versions)
            {
                var verify = ApplicationHelper.VerifyStructure(v);
                if (verify.Code != System.Net.HttpStatusCode.OK)
                    return verify;
            }
            return await HttpHelper.Post(url, data);

        }

        public async Task<ActionResponse> RegisterApplicationVersion(string id, ModuleVersion v)
        {

            var url = $"{_Applications}/{id}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);
            v.ApplicationName = application.ApplicationName;
            if (application.Versions.FirstOrDefault(c=>c.VersionId == v.VersionId) != null)
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Version Already Exists"
                };

            url = $"{_Applications}/{id}/Version";

            var verify = ApplicationHelper.VerifyStructure(v);
            if (verify.Code != System.Net.HttpStatusCode.OK)
                return verify;

            return await HttpHelper.Post(url, v);
        }
        public async Task<ActionResponse> UpdateApplicationVersion(string ApplicationId, string VersionId, ModuleVersion data)
        {
            var url = $"{_Applications}/{ApplicationId}/Version/{VersionId}";

            data.ApplicationName = ApplicationId;
            var verify = ApplicationHelper.VerifyStructure(data);
            if (verify.Code != System.Net.HttpStatusCode.OK)
                return verify;
            return await HttpHelper.Put(url, data);
        }
        public async Task<ActionResponse> DeleteApplication(string id)
        {

            var url = $"{_Applications}/{id}";
            return await HttpHelper.Delete(url);
        }

        public async Task<ActionResponse> DeleteApplicationVersion(string id, string VersionId)
        {
            var url = $"{_Applications}/{id}/Version/{VersionId}";
            return await HttpHelper.Delete(url);
        }
        // Patient --------------------------------------------------------------------------------------
        public async Task<ActionResponse> CreatePatient(Patient patient)
        {
            patient.AcceptedTherapists = new HashSet<string>();
            patient.RequestedTherapists = new HashSet<string>();
            patient.Modules = new HashSet<Module>();

            var url = $"{_Patients}";
            return await HttpHelper.Post(url, patient);
        }
        public async Task<ActionResponse> UpdatePatient(string Email, Patient data)
        {
            var url = $"{_Patients}/{Email}";
            var patientString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(patientString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var existingPatient = JsonConvert.DeserializeObject<Patient>(patientString);
            if (string.IsNullOrEmpty(data.FirstName)) existingPatient.FirstName = data.FirstName;
            if (string.IsNullOrEmpty(data.LastName)) existingPatient.LastName = data.LastName;
            if (string.IsNullOrEmpty(data.PhoneNumber)) existingPatient.PhoneNumber = data.PhoneNumber;
            if (string.IsNullOrEmpty(data.Address)) existingPatient.Address = data.Address;
            if (data.Age != 0 && existingPatient.Age != data.Age) existingPatient.Age = data.Age;
            if (string.IsNullOrEmpty(data.ConditionName)) existingPatient.ConditionName = data.ConditionName;
            if (string.IsNullOrEmpty(data.ProfilePicture)) existingPatient.ProfilePicture = data.ProfilePicture;
            if (data.ConditionAcquisitionDate != DateTime.MinValue) existingPatient.ConditionAcquisitionDate = data.ConditionAcquisitionDate;
            

            url = $"{_Patients}/{Email}";
            return await HttpHelper.Put(url, existingPatient);
        }
        public async Task<ActionResponse> UpdatePatientModuleToVersion(string Email, Guid ModuleId, string VersionId)
        {
        
            var url = $"{_Patients}/{Email}";
            // Find Module By Id
            var patientString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(patientString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Patient Not Found"
                };
            var data = JsonConvert.DeserializeObject<Patient>(patientString);
            var module = data.Modules.FirstOrDefault(c => c.Id == ModuleId);
            if (module == null)
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Module Not Found"
                };

            // Find Application By Id
            url = $"{_Applications}/{module.ModuleData.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);
            // Find Version By Id
            var version = application.Versions.FirstOrDefault(c => c.VersionId == VersionId);
            if (version == null)
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Version Not Found"
                };
            // Merge Data Structures
            var mergedDataStructures = new List<DataPoint>();
            foreach (var dataPoint in version.DataStructure)
            {
                var commonSections = module.ModuleData.DataStructure.FirstOrDefault(c => c.SectionName == dataPoint.SectionName);
                if (commonSections != null && dataPoint.isDataEditable == true)
                {
                    dataPoint.Content = JsonHelper.MergeJsonStructures(commonSections.Content.ToString(), dataPoint.Content.ToString());
                }
                else
                {
                    mergedDataStructures.Add(dataPoint);
                }
            }
            module.ModuleData.HtmlCard = version.HtmlCard;
            module.ModuleData.HtmlDashboard = version.HtmlDashboard;
            module.ModuleData.DataStructure = mergedDataStructures;
            module.ModuleData.VersionId = VersionId;
            module.ModuleData.Timestamp = DateTime.UtcNow;

            var validateModule = ApplicationHelper.VerifyModuleStructure(application, module.ModuleData);
            if (validateModule.Code != System.Net.HttpStatusCode.OK)
                return validateModule;

            url = $"{_Patients}/{Email}/Modules/{ModuleId}";
            return await HttpHelper.Put(url, data);
        }
        public async Task<ActionResponse> AddExistingModuleToPatient(string Email, string id)
        {
            var url = $"{_Patients}/{Email}/Modules/{id}";
            return await HttpHelper.Post(url, "");
        }
        public async Task<ActionResponse> AddModuleToPatient(string Email, Module data)
        {
            var url = $"{_Applications}/{data.ModuleData.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);


            data.ModuleData = application.Versions.OrderByDescending(s => s.Timestamp).FirstOrDefault();



            url = $"{_Patients}/{Email}/Modules";
            return await HttpHelper.Post(url, data);
        }

       
        public async Task<ActionResponse> UpdatePatientModule(string Email, Guid ModuleId, Module data)
        {
            var url = $"{_Patients}/{Email}/Modules/{ModuleId}";
            var moduleString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(moduleString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var module = JsonConvert.DeserializeObject<Module>(moduleString);

            var verify = ApplicationHelper.VerifyModuleContent(data.ModuleData, module.ModuleData);
            if (verify.Code != System.Net.HttpStatusCode.OK)
                return verify;

            data.ModuleData.VersionId = module.ModuleData.VersionId;
            data.ModuleData.ApplicationName = module.ModuleData.ApplicationName;

            url = $"{_Applications}/{data.ModuleData.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);

            data.ModuleData.ApplicationName = application.ApplicationName;

            var validateModule = ApplicationHelper.VerifyModuleStructure(application, data.ModuleData);
            if (validateModule.Code != System.Net.HttpStatusCode.OK)
                return validateModule;
          
            url = $"{_Patients}/{Email}/Modules/{ModuleId}";

            return await HttpHelper.Put(url, data);
        }

        public async Task<ActionResponse> PatientRejectTherapist(string Email, string Therapist)
        {

            var url = $"{_Patients}/{Email}/Therapist/{Therapist}";
            return await HttpHelper.Delete(url);
        }

        public async Task<ActionResponse> PatientRequestTherapist(string Email, string Therapist)
        {

            var url = $"{_Patients}/{Email}/Therapist/{Therapist}";
            return await HttpHelper.Put(url, "");
        }

        public async Task<ActionResponse> DeletePatient(string Email)
        {
            var url = $"{_Patients}/{Email}";
            return await HttpHelper.Delete(url);
        }
        public async Task<ActionResponse> DeletePatientModule(string Email, Guid ModuleId)
        {
            var url = $"{_Patients}/{Email}/Modules/{ModuleId}";
            return await HttpHelper.Delete(url);
        }


        // Therapist ------------------------------------------------------------------------------------
        public async Task<ActionResponse> CreateTherapist(Therapist therapist)
        {

            var url = $"{_Therapists}";
            return await HttpHelper.Post(url, therapist);
        }
        public async Task<ActionResponse> UpdateTherapist(string Email, Therapist data)
        {
            var url = $"{_Therapists}/{Email}";
            var therapistString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(therapistString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Therapist Not Found"
                };
            var existingTherapist = JsonConvert.DeserializeObject<Therapist>(therapistString);
            if (string.IsNullOrEmpty(data.FirstName)) existingTherapist.FirstName = data.FirstName;
            if (string.IsNullOrEmpty(data.LastName)) existingTherapist.LastName = data.LastName;
            if (data.Age != 0 && existingTherapist.Age != data.Age) existingTherapist.Age = data.Age;
            if (string.IsNullOrEmpty(data.PhoneNumber)) existingTherapist.PhoneNumber = data.PhoneNumber;
            if (string.IsNullOrEmpty(data.Address)) existingTherapist.Address = data.Address;
            if (string.IsNullOrEmpty(data.Credentials)) existingTherapist.Credentials = data.Credentials;
            if (string.IsNullOrEmpty(data.Description)) existingTherapist.Description = data.Description;
            if (string.IsNullOrEmpty(data.ProfilePicture)) existingTherapist.ProfilePicture = data.ProfilePicture;
            

            url = $"{_Therapists}/{Email}";
            return await HttpHelper.Put(url, existingTherapist);
        }
        public async Task<ActionResponse> TherapistAcceptPatient(string Email, string Patient)
        {
            var url = $"{_Therapists}/{Email}/Patient/{Patient}";
            return await HttpHelper.Put(url, "");
        }

        public async Task<ActionResponse> TherapistRejectPatient(string Email, string Patient)
        {
            var url = $"{_Therapists}/{Email}/Patient/{Patient}";
            return await HttpHelper.Delete(url);
        }
        public async Task<ActionResponse> DeleteTherapist(string Email)
        {
            var url = $"{_Therapists}/{Email}";
            return await HttpHelper.Delete(url);
        }

        // Modules --------------------------------------------------------------------------------------
        public async Task<ActionResponse> CreateModule(Module data)
        {
            var url = $"{_Applications}/{data.ModuleData.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);
            
            data.ModuleData = application.Versions.OrderByDescending(s => s.Timestamp).FirstOrDefault();
           
            url = $"{_Modules}";
            return await HttpHelper.Post(url, data);
        }

        public async Task<ActionResponse> UpdateModuleToVersion(string id, string VersionId)
        {
            var url = $"{_Modules}/{id}";
            // Find Module By Id
            var moduleString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(moduleString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Module Not Found"
                };
            var data = JsonConvert.DeserializeObject<Module>(moduleString);

            // Find Application By Id
            url = $"{_Applications}/{data.ModuleData.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);
            // Find Version By Id
            var version = application.Versions.FirstOrDefault(c => c.VersionId == VersionId);
            if (version == null)
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Version Not Found"
                };
            // Merge Data Structures
            var mergedDataStructures = new List<DataPoint>();
            foreach(var dataPoint in version.DataStructure)
            {
                var commonSections = data.ModuleData.DataStructure.FirstOrDefault(c => c.SectionName == dataPoint.SectionName);
                if (commonSections != null && dataPoint.isDataEditable == true)
                {
                    dataPoint.Content = JsonHelper.MergeJsonStructures(commonSections.Content.ToString(), dataPoint.Content.ToString());
                    mergedDataStructures.Add(dataPoint);
                }
                else
                {
                    mergedDataStructures.Add(dataPoint);
                }
            }
            data.ModuleData.DataStructure = mergedDataStructures;
            data.ModuleData.HtmlCard = version.HtmlCard;
            data.ModuleData.HtmlDashboard = version.HtmlDashboard;
            data.ModuleData.VersionId = VersionId;
            data.ModuleData.Timestamp = DateTime.UtcNow;

            var validateModule = ApplicationHelper.VerifyModuleStructure(application, data.ModuleData);
            if (validateModule.Code != System.Net.HttpStatusCode.OK)
                return validateModule;

            url = $"{_Modules}/{id}";
            return await HttpHelper.Put(url, data);

        }


        public async Task<ActionResponse> UpdateModule(string id, ModuleVersion data)
        {
            var url = $"{_Modules}/{id}";
            var moduleString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(moduleString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Module Not Found"
                };
            var module = JsonConvert.DeserializeObject<Module>(moduleString);

            var verify = ApplicationHelper.VerifyModuleContent(data, module.ModuleData);
            if (verify.Code != System.Net.HttpStatusCode.OK)
                return verify;

            data.VersionId = module.ModuleData.VersionId;
            data.ApplicationName = module.ModuleData.ApplicationName;

            url = $"{_Applications}/{data.ApplicationName}";
            var applicationString = await HttpHelper.Get(url);
            if (string.IsNullOrEmpty(applicationString))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Application Not Found"
                };
            var application = JsonConvert.DeserializeObject<Application>(applicationString);
            data.ApplicationName = application.ApplicationName;

            var validateModule = ApplicationHelper.VerifyModuleStructure(application, data);

            if (validateModule.Code != System.Net.HttpStatusCode.OK)
                return validateModule;

            verify = ApplicationHelper.VerifyStructure(data);
            if (verify.Code != System.Net.HttpStatusCode.OK)
                return verify;

            url = $"{_Modules}/{id}";
         
            var updatedModule = new Module
            {
                Id = Guid.Parse(id),
                ModuleData = data
            };
            return await HttpHelper.Put(url, updatedModule);
        }

        public async Task<ActionResponse> DeleteModule(string id)
        {
            var url = $"{_Modules}/{id}";
            return await HttpHelper.Delete(url);
        }

       
    }
}
