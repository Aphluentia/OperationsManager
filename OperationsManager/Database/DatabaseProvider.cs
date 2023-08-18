using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using DatabaseApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OperationsManager.Configurations;
using OperationsManager.Helpers;
using System;

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

        public async Task<bool> AddExistingModuleToPatient(string Email, string id)
        {
            var url = $"{_Patients}/{Email}/Modules/{id}";
            return await DatabaseOperationsHelper.Post(url, "");
        }

        public async Task<bool> AddModuleToPatient(string Email, Module data)
        {
            var url = $"{_Patients}/{Email}/Modules";
            return await DatabaseOperationsHelper.Post(url, data);
        }

        public async Task<bool> CreateModule(Module module)
        {
            var url = $"{_Modules}";
            return await DatabaseOperationsHelper.Post(url, module);
        }

        public async Task<bool> CreatePatient(Patient patient)
        {

            var url = $"{_Patients}";
            return await DatabaseOperationsHelper.Post(url, patient);
        }

        public async Task<bool> CreateTherapist(Therapist therapist)
        {

            var url = $"{_Therapists}";
            return await DatabaseOperationsHelper.Post(url, therapist);
        }

        public async Task<bool> DeleteApplication(string id)
        {

            var url = $"{_Applications}/{id}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> DeleteApplicationVersion(string id, string VersionId)
        {
            var url = $"{_Applications}/{id}/Version/{VersionId}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> DeleteModule(string id)
        {
            var url = $"{_Modules}/{id}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> DeletePatient(string Email)
        {
            var url = $"{_Patients}/{Email}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> DeletePatientModule(string Email, string ModuleId)
        {
            var url = $"{_Patients}/{Email}/Modules/{ModuleId}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> DeleteTherapist(string Email)
        {
            var url = $"{_Therapists}/{Email}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> PatientRejectTherapist(string Email, string Therapist)
        {

            var url = $"{_Patients}/{Email}/Therapist/{Therapist}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> PatientRequestTherapist(string Email, string Therapist)
        {

            var url = $"{_Patients}/{Email}/Therapist/{Therapist}";
            return await DatabaseOperationsHelper.Put(url, "");
        }

        public async Task<bool> RegisterApplication(Application data)
        {

            var url = $"{_Applications}";
            return await DatabaseOperationsHelper.Post(url, data);
        }

        public async Task<bool> RegisterApplicationVersion(string id, ModuleVersion data)
        {

            var url = $"{_Applications}/{id}/Version";
            return await DatabaseOperationsHelper.Post(url, data);
        }

        public async Task<bool> TherapistAcceptPatient(string Email, string Patient)
        {
            var url = $"{_Therapists}/{Email}/Patient/{Patient}";
            return await DatabaseOperationsHelper.Put(url, "");
        }

        public async Task<bool> TherapistRejectPatient(string Email, string Patient)
        {
            var url = $"{_Therapists}/{Email}/Patient/{Patient}";
            return await DatabaseOperationsHelper.Delete(url);
        }

        public async Task<bool> UpdateApplicationVersion(string ApplicationId, string VersionId, ModuleVersion data)
        {
            var url = $"{_Applications}/{ApplicationId}/Version/{VersionId}";
            return await DatabaseOperationsHelper.Put(url, data);
        }

        public async Task<bool> UpdateModule(string id, Module data)
        {
            var url = $"{_Modules}/{id}";
            return await DatabaseOperationsHelper.Put(url, data);
        }

        public async Task<bool> UpdateModuleToVersion(string id, string VersionId)
        {
            var url = $"{_Modules}/{id}/Version/{VersionId}";
            return await DatabaseOperationsHelper.Put(url, "");
        }

        public async Task<bool> UpdateModuleVersion(string id, ModuleVersion data)
        {
            var url = $"{_Modules}/{id}/Version";
            return await DatabaseOperationsHelper.Put(url, data);
        }

        public async Task<bool> UpdatePatient(string Email, Patient data)
        {
            var url = $"{_Patients}/{Email}";
            return await DatabaseOperationsHelper.Put(url, data);
        }

        public async Task<bool> UpdatePatientModule(string Email, string ModuleId, Module data)
        {
            var url = $"{_Patients}/{Email}/Modules/{ModuleId}";
            return await DatabaseOperationsHelper.Put(url, data);
        }

        public async Task<bool> UpdatePatientModuleToVersion(string Email, string ModuleId, string VersionId)
        {
            var url = $"{_Patients}/{Email}/Modules/{ModuleId}?Version={VersionId}";
            return await DatabaseOperationsHelper.Put(url, "");
        }

        public async Task<bool> UpdateTherapist(string Email, Therapist data)
        {
            var url = $"{_Therapists}/{Email}";
            return await DatabaseOperationsHelper.Put(url, data);
        }
    }
}
