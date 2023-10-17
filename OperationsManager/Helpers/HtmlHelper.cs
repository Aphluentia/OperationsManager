using HtmlAgilityPack;
namespace OperationsManager.Helpers
{
    public class HtmlHelper
    {
        public static bool IsValidHtml(string htmlString)
        {
            HtmlDocument htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(htmlString);

            if (htmlDocument.ParseErrors != null && htmlDocument.ParseErrors.Count() > 0)
            {
                return false;
            }
            return true;
        }
    }
}
