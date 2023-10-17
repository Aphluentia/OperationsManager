using Amazon.Auth.AccessControlPolicy.ActionIdentifiers;
using DatabaseApi.Models.Entities;
using OperationsManager.Database.Entities;
using System.Text.Json;

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
            var dataStructureSections = dataStructure.DataStructure.Select(c => c.SectionName).ToList();
            var moduleDataStructureSections = moduleDataStructure.Select(c => c.SectionName).ToList();
            
            if (!dataStructureSections.All(c=>moduleDataStructureSections.Contains(c)) || dataStructureSections.Count() != moduleDataStructureSections.Count() )
            {
                return new ActionResponse
                {
                    Code = System.Net.HttpStatusCode.BadRequest,
                    Message = "Incorrect Data Structure"
                };
            }
            foreach(DataPoint section in dataStructure.DataStructure)
            {
                var moduleSection = moduleDataStructure.FirstOrDefault(c => c.SectionName == section.SectionName);
                if (!JsonHelper.IsJsonStructure(section.Content.ToString()) || !JsonHelper.IsJsonStructure(moduleSection.Content.ToString()))
                {
                    return new ActionResponse
                    {
                        Code = System.Net.HttpStatusCode.BadRequest,
                        Message = "Json is Invalid"
                    };
                }
           
                if (!section.isDataEditable && section.Content!=moduleSection.Content)
                {
                    return new ActionResponse
                    {
                        Code = System.Net.HttpStatusCode.BadRequest,
                        Message = $"Content of Section {section.SectionName} is not Editable"
                    };
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

    }
}
