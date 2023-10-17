using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace OperationsManager.Helpers
{
    public class JsonHelper
    {
        public static bool IsJsonStructure(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        public static string MergeJsonStructures(string json1, string json2)
        {
            JObject obj1 = JObject.Parse(json1);
            JObject obj2 = JObject.Parse(json2);

            obj1.Merge(obj2, new JsonMergeSettings
            {
                // Use MergeArrayHandling.Ignore or MergeArrayHandling.Concat based on your requirement
                MergeArrayHandling = MergeArrayHandling.Concat,
                // Use MergeNullValueHandling.Merge
                MergeNullValueHandling = MergeNullValueHandling.Merge,
            });

            return obj1.ToString();
        }
    }
}
