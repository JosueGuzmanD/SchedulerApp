using SchedulerApplication.Common.Enums;
using System.Globalization;
using System.Resources;

namespace SchedulerApplication.Services
{
    public static class CultureManager
    {
        public static void SetCulture(CultureOptions cultureOption)
        {
            var cultureInfo = cultureOption switch
            {
                CultureOptions.EnUs => new CultureInfo("en-US"),
                CultureOptions.EnGB=> new CultureInfo("en-GB"),
                CultureOptions.EsEs => new CultureInfo("es-ES"),
                _ => new CultureInfo("en-GB"), // En-GB como predeterminado
            };

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;
        }

        public static string GetLocalizedString(string key)
        {
            string cultureName = CultureInfo.CurrentUICulture.Name.Replace("-", "-");
            string resourceBaseName = $"SchedulerApplication.Common.Cultures.Culture{cultureName}";
            var resourceManager = new ResourceManager(resourceBaseName, typeof(CultureManager).Assembly);
            return resourceManager.GetString(key, CultureInfo.CurrentUICulture);
        }

        public static string FormatDate(DateTime dateTime)
        {
            var format = CultureInfo.CurrentCulture.Name switch
            {
                "en-US" => "MM/dd/yyyy",
                _ => "dd/MM/yyyy",
            };
            return dateTime.ToString(format, CultureInfo.CurrentCulture);
        }

        public static string FormatTime(TimeSpan timeSpan)
        {
            return timeSpan.ToString(@"hh\:mm", CultureInfo.CurrentCulture);
        }

    }

}
