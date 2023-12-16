using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using DatabaseApi.Models.Entities;
using OperationsManager.Database.Entities;
using System;
using System.Text.Json;
using static System.Collections.Specialized.BitVector32;

namespace OperationsManager.Helpers
{
    public class ApplicationHelper
    {
        public static ActionResponse VerifyStructure(ModuleVersion v)
        {
            
            foreach (DataPoint dataPoint in v.DataStructure)
            {
                if ((!string.IsNullOrEmpty(dataPoint.Content.ToString()) && !JsonHelper.IsJsonStructure(dataPoint.Content.ToString())) ||string.IsNullOrEmpty(dataPoint.Content))
                {
                    return new ActionResponse
                    {
                        Code = System.Net.HttpStatusCode.BadRequest,
                        Message = "Failed to Validate Application Data as Json Structure"
                    };
                }

            }
            if (!string.IsNullOrEmpty(v.HtmlCard) && !HtmlHelper.IsValidHtml(v.HtmlCard))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Card Template is Not a Valid HTML structure"
                };

            if (!string.IsNullOrEmpty(v.HtmlDashboard) && !HtmlHelper.IsValidHtml(v.HtmlDashboard))
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Dashboard Template is Not a Valid HTML structure"
                };
            return new ActionResponse
            {
                Code = System.Net.HttpStatusCode.OK
            };
        }
        public static ActionResponse VerifyModuleStructure(Application app, ModuleVersion module)
        {

            var dataStructure = app.Versions.FirstOrDefault(c => c.VersionId == module.VersionId);
            if (dataStructure == null)
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.NotFound,
                    Message = "Application Version Not Found"
                };

            var moduleDataStructure = module.DataStructure;
            var dataStructureSections = dataStructure.DataStructure.Select(c => c.SectionName).ToHashSet<string>();
            var moduleDataStructureSections = moduleDataStructure.Select(c => c.SectionName).ToHashSet<string>();
            
            if (!dataStructureSections.All(c=>moduleDataStructureSections.Contains(c)) || dataStructureSections.Count() < moduleDataStructureSections.Count() )
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Incorrect Data Structure"
                };
            }

           
            foreach(DataPoint section in dataStructure.DataStructure)
            {

                var moduleSections = moduleDataStructure.Where(c => c.SectionName == section.SectionName);
                foreach(var sectionContext in moduleSections)
                {
                    if (!JsonHelper.IsJsonStructure(sectionContext.Content.ToString()) || !JsonHelper.IsJsonStructure(section.Content.ToString()))
                    {
                        return new ActionResponse
                        {
                            Code = System.Net.HttpStatusCode.BadRequest,
                            Message = "Json is Invalid"
                        };
                    }
                    if (!section.isDataEditable && section.Content != sectionContext.Content)
                    {
                        return new ActionResponse
                        {
                            Code = System.Net.HttpStatusCode.BadRequest,
                            Message = $"Content of Section {section.SectionName} is not Editable"
                        };
                    }
                }
            }
            return new ActionResponse
            {
                Code = System.Net.HttpStatusCode.OK,
                Message = ""
            };
        }

        public static ActionResponse VerifyModuleContent(ModuleVersion updated, ModuleVersion existing)
        {
            
            if (existing.Checksum != updated.Checksum && updated.Timestamp < existing.Timestamp)
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Not Updated Because Given Data is Older that Registered in Database"
                };
            }
            return new ActionResponse
            {
                Code = System.Net.HttpStatusCode.OK,
                Message = ""
            };
        }

        public static ActionResponse VerifyContext(ModuleVersion version)
        {
            var SectionsAndContexts = version.DataStructure.ToList().Select(c => (c.SectionName, c.ContextName));
            var duplicates = SectionsAndContexts.GroupBy(c => c)
                                        .Where(group => group.Count() > 1)
                                        .Select(group => group.Key)
                                        .ToList();
            if (duplicates.Count() > 0)
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Not Updated Because There are Repeated Data Entries"
                };
            }
            if (!SectionsAndContexts.Any(c=>c.ContextName == version.ActiveContextName))
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Active Context Name Doesnt Exist"
                };
            }
            
            return new ActionResponse
            {
                Code = System.Net.HttpStatusCode.OK,
                Message = ""
            };
        }

        public static ActionResponse VerifyDispairingContexts(ModuleVersion v)
        {
            
            var SectionsAndContexts = v.DataStructure.ToList().Select(c=>(c.SectionName, c.ContextName)).ToList();
            var occurrenceCounts = SectionsAndContexts
                                .GroupBy(entry => entry)
                                .ToDictionary(group => group.Key, group => group.Count());
            if (occurrenceCounts.Any(c=>c.Value!=occurrenceCounts.FirstOrDefault().Value)) 
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Not all Sections are present in different profiles"
                };
            }

            var SectionsAndIsEditable = v.DataStructure.ToList().Select(c => (c.SectionName, c.isDataEditable)).ToList();
            var inconsistentSections = SectionsAndIsEditable
                                .GroupBy(entry => entry.SectionName)
                                .Where(group => group.Select(entry => entry.isDataEditable).Distinct().Count() > 1)
                                .Select(group => group.Key)
                                .ToList();
            if (inconsistentSections.Any())
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Sections in Different Profiles Can't Have different editability configurations"
                };
            }

            return new ActionResponse
            {
                Code = System.Net.HttpStatusCode.OK,
                Message = ""
            };
        }
    }
}
