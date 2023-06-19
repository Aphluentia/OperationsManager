
using OperationsManager.Database;
using OperationsManager.Models.Enums;
using System.ComponentModel;
using Newtonsoft.Json;
using DatabaseApi.Models.Entities;
using OperationsManager.Models.BrokerMessageDataField;
using OperationsManager.Helpers;
using Amazon.Runtime.Internal;

namespace OperationsManager.BackgroundServices.Workers
{
    public class TMessageHandler
    {
        private BackgroundWorker _worker;
        private bool _StopFlag;
        private readonly IKafkaMessageHandler _kafkaMonitor;
        private readonly IDatabaseProvider _database;
        public TMessageHandler(KafkaQueue mKafka, IDatabaseProvider database)
        {
            _kafkaMonitor = mKafka;
            _database = database;
            _StopFlag = false;
            _worker = new BackgroundWorker();
            _worker.DoWork += RunAsync;
        }

        public void Start()
        {
            _worker.RunWorkerAsync();
        }

        private async void RunAsync(object sender, DoWorkEventArgs e)
        {
            LogHelper.Initialize("C:\\Users\\E1493\\OneDrive - Default Directory\\Desktop\\log.txt");
            while (!_StopFlag)
            {
                var message = this._kafkaMonitor.FetchIncomingMessage();
                var stringData = message.Data.ToString();
                bool success = false;
                if (message != null && !string.IsNullOrEmpty(stringData))
                {
                    switch (message.OperationCode)
                    {
                        case Operation.REGISTER_APPLICATION:
                            var REGISTER_APPLICATION = JsonConvert.DeserializeObject<Application>(stringData);
                            if (REGISTER_APPLICATION != null)
                                success = await _database.RegisterApplication(REGISTER_APPLICATION);
                            LogHelper.WriteLog($"Performing register application operation with success:{success}");
                            break;
                        case Operation.ADD_APPLICATION_VERSION:
                            var ADD_APPLICATION_VERSION = JsonConvert.DeserializeObject<UpdateDto<ModuleVersion>>(stringData);

                            if (ADD_APPLICATION_VERSION != null)
                                success = await _database.RegisterApplicationVersion(ADD_APPLICATION_VERSION.Id, ADD_APPLICATION_VERSION.Data);
                            LogHelper.WriteLog($"Performing register application version operation with success:{success}");
                            break;
                        case Operation.UPDATE_APPLICATION_VERSION:
                            var UPDATE_APPLICATION_VERSION = JsonConvert.DeserializeObject<UpdateDto<ModuleVersion>>(stringData);

                            if (UPDATE_APPLICATION_VERSION != null)
                                success = await _database.UpdateApplicationVersion(UPDATE_APPLICATION_VERSION.Id, UPDATE_APPLICATION_VERSION.Id2, UPDATE_APPLICATION_VERSION.Data);
                            LogHelper.WriteLog($"Performing update application version operation with success:{success}");
                            break;
                        case Operation.DELETE_APPLICATION_VERSION:
                            var DELETE_APPLICATION_VERSION = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (DELETE_APPLICATION_VERSION != null)
                                success = await _database.DeleteApplicationVersion(DELETE_APPLICATION_VERSION.Id, DELETE_APPLICATION_VERSION.Id2);
                            LogHelper.WriteLog($"Performing delete application version operation with success:{success}");
                            break;
                        case Operation.DELETE_APPLICATION:
                            var DELETE_APPLICATION = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (DELETE_APPLICATION != null)
                                success = await _database.DeleteApplication(DELETE_APPLICATION.Id);
                            LogHelper.WriteLog($"Performing delete application operation with success:{success}");
                            break;



                        case Operation.CREATE_MODULE:
                            var CREATE_MODULE = JsonConvert.DeserializeObject<Module>(stringData);

                            if (CREATE_MODULE != null)
                                success = await _database.CreateModule(CREATE_MODULE);
                            LogHelper.WriteLog($"Performing create module operation with success:{success}");
                            break;
                        case Operation.UPDATE_MODULE:
                            var UPDATE_MODULE = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (UPDATE_MODULE != null)
                                success = await _database.UpdateModule(UPDATE_MODULE.Id, UPDATE_MODULE.Data);
                            LogHelper.WriteLog($"Performing update module operation with success:{success}");
                            break;
                        case Operation.UPDATE_MODULE_TO_VERSION:
                            var UPDATE_MODULE_TO_VERSION = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (UPDATE_MODULE_TO_VERSION != null)
                                success = await _database.UpdateModuleToVersion(UPDATE_MODULE_TO_VERSION.Id, UPDATE_MODULE_TO_VERSION.Id2);
                            LogHelper.WriteLog($"Performing update module to version operation with success:{success}");
                            break;
                        case Operation.UPDATE_MODULE_VERSION:
                            var UPDATE_MODULE_VERSION = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (UPDATE_MODULE_VERSION != null)
                                success = await _database.UpdateModuleVersion(UPDATE_MODULE_VERSION.Id, UPDATE_MODULE_VERSION.Data);
                            LogHelper.WriteLog($"Performing update module version operation with success:{success}");
                            break;
                        case Operation.DELETE_MODULE:
                            var DELETE_MODULE = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (DELETE_MODULE != null)
                                success = await _database.DeleteModule(DELETE_MODULE.Id);
                            LogHelper.WriteLog($"Performing delete module operation with success:{success}");
                            break;
                        case Operation.CREATE_PATIENT:
                            var CREATE_PATIENT = JsonConvert.DeserializeObject<Patient>(stringData);

                            if (CREATE_PATIENT != null)
                                success = await _database.CreatePatient(CREATE_PATIENT);
                            LogHelper.WriteLog($"Performing create patient operation with success:{success}");
                            break;
                        case Operation.UPDATE_PATIENT:
                            var UPDATE_PATIENT = JsonConvert.DeserializeObject<UpdateDto<Patient>>(stringData);

                            if (UPDATE_PATIENT != null)
                                success = await _database.UpdatePatient(UPDATE_PATIENT.Id, UPDATE_PATIENT.Data);
                            LogHelper.WriteLog($"Performing update patient operation with success:{success}");
                            break;
                        case Operation.DELETE_PATIENT:
                            var DELETE_PATIENT = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (DELETE_PATIENT != null)
                                success = await _database.DeletePatient(DELETE_PATIENT.Id);
                            LogHelper.WriteLog($"Performing delete patient operation with success:{success}");
                            break;
                        case Operation.ADD_PATIENT_EXISTING_MODULE:
                            var ADD_PATIENT_EXISTING_MODULE = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (ADD_PATIENT_EXISTING_MODULE != null)
                                success = await _database.AddExistingModuleToPatient(ADD_PATIENT_EXISTING_MODULE.Id, ADD_PATIENT_EXISTING_MODULE.Id2);
                            LogHelper.WriteLog($"Performing add existing module to patient operation with success:{success}");
                            break;
                        case Operation.ADD_PATIENT_NEW_MODULE:
                            var ADD_PATIENT_NEW_MODULE = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (ADD_PATIENT_NEW_MODULE != null)
                                success = await _database.AddModuleToPatient(ADD_PATIENT_NEW_MODULE.Id, ADD_PATIENT_NEW_MODULE.Data);
                            LogHelper.WriteLog($"Performing add module to patient operation with success:{success}");
                            break;
                        case Operation.UPDATE_PATIENT_MODULE:
                            var UPDATE_PATIENT_MODULE = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (UPDATE_PATIENT_MODULE != null)
                                success = await _database.UpdatePatientModule(UPDATE_PATIENT_MODULE.Id, UPDATE_PATIENT_MODULE.Id2, UPDATE_PATIENT_MODULE.Data);
                            LogHelper.WriteLog($"Performing update patient module operation with success:{success}");
                            break;
                        case Operation.UPDATE_PATIENT_MODULE_VERSION:
                            var UPDATE_PATIENT_MODULE_VERSION = JsonConvert.DeserializeObject<UpdateDto<Module>>(stringData);

                            if (UPDATE_PATIENT_MODULE_VERSION != null)
                                success = await _database.UpdatePatientModuleToVersion(UPDATE_PATIENT_MODULE_VERSION.Id, UPDATE_PATIENT_MODULE_VERSION.Id2, UPDATE_PATIENT_MODULE_VERSION.Id3);
                            LogHelper.WriteLog($"Performing update patient module to version operation with success:{success}");
                            break;
                        case Operation.DELETE_PATIENT_MODULE:
                            var DELETE_PATIENT_MODULE = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (DELETE_PATIENT_MODULE != null)
                                success = await _database.DeletePatientModule(DELETE_PATIENT_MODULE.Id, DELETE_PATIENT_MODULE.Id2);
                            LogHelper.WriteLog($"Performing delete patient module operation with success:{success}");
                            break;
                        case Operation.PATIENT_REQUEST_THERAPIST:
                            var PATIENT_REQUEST_THERAPIST = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (PATIENT_REQUEST_THERAPIST != null)
                                success = await _database.PatientRequestTherapist(PATIENT_REQUEST_THERAPIST.Id, PATIENT_REQUEST_THERAPIST.Id2);
                            LogHelper.WriteLog($"Performing patient request therapist operation with success:{success}");
                            break;
                        case Operation.PATIENT_REJECT_THERAPIST:
                            var PATIENT_REJECT_THERAPIST = JsonConvert.DeserializeObject<DeleteDto>(stringData);

                            if (PATIENT_REJECT_THERAPIST != null)
                                success = await _database.PatientRejectTherapist(PATIENT_REJECT_THERAPIST.Id, PATIENT_REJECT_THERAPIST.Id2);
                            LogHelper.WriteLog($"Performing patient reject therapist operation with success:{success}");
                            break;
                        case Operation.CREATE_THERAPIST:
                            var CREATE_THERAPIST = JsonConvert.DeserializeObject<Therapist>(stringData);
                            if (CREATE_THERAPIST != null) 
                                success = await _database.CreateTherapist(CREATE_THERAPIST);
                            LogHelper.WriteLog($"Performing create therapist operation with success:{success}");
                            break;
                        case Operation.UPDATE_THERAPIST:
                            var UPDATE_THERAPIST = JsonConvert.DeserializeObject<UpdateDto<Therapist>>(stringData);
                            if (UPDATE_THERAPIST != null)
                                success = await _database.UpdateTherapist(UPDATE_THERAPIST.Id, UPDATE_THERAPIST.Data);
                            LogHelper.WriteLog($"Performing update therapist operation with success:{success}");
                            break;
                        case Operation.DELETE_THERAPIST:
                            var DELETE_THERAPIST = JsonConvert.DeserializeObject<DeleteDto>(stringData);
                            if (DELETE_THERAPIST != null)
                                success = await _database.DeleteTherapist(DELETE_THERAPIST.Id);
                            LogHelper.WriteLog($"Performing delete therapist operation with success:{success}");
                            break;
                        case Operation.THERAPIST_ACCEPT_PATIENT:
                            var THERAPIST_ACCEPT_PATIENT = JsonConvert.DeserializeObject<DeleteDto>(stringData);
                            if (THERAPIST_ACCEPT_PATIENT != null)
                                success = await _database.TherapistAcceptPatient(THERAPIST_ACCEPT_PATIENT.Id, THERAPIST_ACCEPT_PATIENT.Id2);
                            LogHelper.WriteLog($"Performing therapist accept patient operation with success:{success}");
                            break;
                        case Operation.THERAPIST_REJECT_PATIENT:
                            var THERAPIST_REJECT_PATIENT = JsonConvert.DeserializeObject<DeleteDto>(stringData);
                            if (THERAPIST_REJECT_PATIENT != null)
                                success = await _database.TherapistRejectPatient(THERAPIST_REJECT_PATIENT.Id, THERAPIST_REJECT_PATIENT.Id2);
                            LogHelper.WriteLog($"Performing therapist reject patient operation with success:{success}");
                            break;
                    
                    }

                  

                }
            }
            LogHelper.Dispose();
            

        }

      
    }
}
