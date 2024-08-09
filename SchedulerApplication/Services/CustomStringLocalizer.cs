using Microsoft.Extensions.Localization;
using System.Globalization;

public class CustomStringLocalizer : IStringLocalizer
{
    private readonly Dictionary<string, Dictionary<string, string>> _localizations = new Dictionary<string, Dictionary<string, string>>();

    public CustomStringLocalizer()
    {
        // Sample localizations for en-GB, en-US, and es-ES
        _localizations["en_GB"] = new Dictionary<string, string>
        {
            { "DailyDescriptionSingle", "Occurs once at {0}. Schedule will be used on {1} at {2} starting on {3}." },
            { "DailyDescription_Range", "Occurs every day from {0} to {1}. Schedule will be used on {2} at {3} starting on {4}." },
            { "ExecutionTimeGeneratorInvalidIntervalExc", "Invalid interval" },
            { "ExecutionTimeGeneratorUnknownConfigurationExc", "Unknown configuration type" },
            { "MonthlyDescription_AnyDay", "Occurs on day {0} of every {1} month(s). Schedule will be used on {2} at {3} starting on {4}." },
            { "MonthlyDescription_DayOption", "Occurs on the {0} {1} of every {2} month(s). Schedule will be used on {3} at {4} starting on {5}." },
            { "MonthlyDescription_SpecificDay", "Occurs on day {0} of every {1} month(s). Schedule will be used on {2} at {3} starting on {4}." },
            { "MonthlySchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for MonthlyDateCalculator." },
            { "OnceDescription", "Occurs once. Schedule will be used on {0} at {1} starting on {2}." },
            { "OnceSchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for OnceDateCalculator." },
            { "OnceSchedulerConfigurationPastConfigurationExc", "Configuration date time cannot be in the past." },
            { "ScheduleTypeFactoryUnsupportedConfigurationExc", "Unsupported configuration type." },
            { "SpecificDayStrategySchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for SpecificDayStrategy." },
            { "WeeklyDescription", "Occurs every {0} week(s) on {1}. Schedule will be used on {2} at {3} starting on {4}." },
            { "First", "first" },
            { "Second", "second" },
            { "Third", "third" },
            { "Fourth", "fourth" },
            { "Last", "last" },
            { "Weekday", "weekday" },
            { "WeekendDay", "weekend day" },
            { "Monday", "Monday" },
            { "Tuesday", "Tuesday" },
            { "Wednesday", "Wednesday" },
            { "Thursday", "Thursday" },
            { "Friday", "Friday" },
            { "Saturday", "Saturday" },
            { "Sunday", "Sunday" }
        };

        _localizations["en_US"] = new Dictionary<string, string>
        {
            { "DailyDescriptionSingle", "Occurs once at {0}. Schedule will be used on {1} at {2} starting on {3}." },
            { "DailyDescription_Range", "Occurs every day from {0} to {1}. Schedule will be used on {2} at {3} starting on {4}." },
            { "ExecutionTimeGeneratorInvalidIntervalExc", "Invalid interval" },
            { "ExecutionTimeGeneratorUnknownConfigurationExc", "Unknown configuration type" },
            { "MonthlyDescription_AnyDay", "Occurs on day {0} of every {1} month(s). Schedule will be used on {2} at {3} starting on {4}." },
            { "MonthlyDescription_DayOption", "Occurs on the {0} {1} of every {2} month(s). Schedule will be used on {3} at {4} starting on {5}." },
            { "MonthlyDescription_SpecificDay", "Occurs on day {0} of every {1} month(s). Schedule will be used on {2} at {3} starting on {4}." },
            { "MonthlySchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for MonthlyDateCalculator." },
            { "OnceDescription", "Occurs once. Schedule will be used on {0} at {1} starting on {2}." },
            { "OnceSchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for OnceDateCalculator." },
            { "OnceSchedulerConfigurationPastConfigurationExc", "Configuration date time cannot be in the past." },
            { "ScheduleTypeFactoryUnsupportedConfigurationExc", "Unsupported configuration type." },
            { "SpecificDayStrategySchedulerConfigurationInvalidConfigurationExc", "Invalid configuration type for SpecificDayStrategy." },
            { "WeeklyDescription", "Occurs every {0} week(s) on {1}. Schedule will be used on {2} at {3} starting on {4}." },
            { "First", "first" },
            { "Second", "second" },
            { "Third", "third" },
            { "Fourth", "fourth" },
            { "Last", "last" },
            { "Weekday", "weekday" },
            { "WeekendDay", "weekend day" },
            { "Monday", "Monday" },
            { "Tuesday", "Tuesday" },
            { "Wednesday", "Wednesday" },
            { "Thursday", "Thursday" },
            { "Friday", "Friday" },
            { "Saturday", "Saturday" },
            { "Sunday", "Sunday" }
        };

        _localizations["es_ES"] = new Dictionary<string, string>
        {
            { "DailyDescriptionSingle", "Ocurre una vez a las {0}. El horario se utilizará el {1} a las {2} a partir del {3}." },
            { "DailyDescription_Range", "Ocurre todos los días de {0} a {1}. El horario se utilizará el {2} a las {3} a partir del {4}." },
            { "ExecutionTimeGeneratorInvalidIntervalExc", "Intervalo no válido" },
            { "ExecutionTimeGeneratorUnknownConfigurationExc", "Tipo de configuración desconocido" },
            { "MonthlyDescription_AnyDay", "Ocurre el día {0} de cada {1} mes(es). El horario se utilizará el {2} a las {3} a partir del {4}." },
            { "MonthlyDescription_DayOption", "Ocurre el {0} {1} de cada {2} mes(es). El horario se utilizará el {3} a las {4} a partir del {5}." },
            { "MonthlyDescription_SpecificDay", "Ocurre el día {0} de cada {1} mes(es). El horario se utilizará el {2} a las {3} a partir del {4}." },
            { "MonthlySchedulerConfigurationInvalidConfigurationExc", "Tipo de configuración no válido para MonthlyDateCalculator." },
            { "OnceDescription", "Ocurre una vez. El horario se utilizará el {0} a las {1} a partir del {2}." },
            { "OnceSchedulerConfigurationInvalidConfigurationExc", "Tipo de configuración no válido para OnceDateCalculator." },
            { "OnceSchedulerConfigurationPastConfigurationExc", "La fecha y hora de configuración no pueden estar en el pasado." },
            { "ScheduleTypeFactoryUnsupportedConfigurationExc", "Tipo de configuración no compatible." },
            { "SpecificDayStrategySchedulerConfigurationInvalidConfigurationExc", "Tipo de configuración no válido para SpecificDayStrategy." },
            { "WeeklyDescription", "Ocurre cada {0} semana(s) el {1}. El horario se utilizará el {2} a las {3} a partir del {4}." },
            { "First", "primer" },
            { "Second", "segundo" },
            { "Third", "tercer" },
            { "Fourth", "cuarto" },
            { "Last", "último" },
            { "Weekday", "día laborable" },
            { "WeekendDay", "día del fin de semana" },
            { "Monday", "lunes" },
            { "Tuesday", "martes" },
            { "Wednesday", "miércoles" },
            { "Thursday", "jueves" },
            { "Friday", "viernes" },
            { "Saturday", "sábado" },
            { "Sunday", "domingo" }
        };
    }

    public LocalizedString this[string name]
    {
        get
        {
            var value = GetString(name);
            return new LocalizedString(name, value ?? name, resourceNotFound: value == null);
        }
    }

    public LocalizedString this[string name, params object[] arguments]
    {
        get
        {
            var format = GetString(name);

            var value = string.Format(CultureInfo.CurrentCulture, format, arguments);
            return new LocalizedString(name, value, resourceNotFound: false);
        }
    }


    private string GetString(string name)
    {
        var cultureName = CultureInfo.CurrentCulture.Name.Replace("-", "_");

        if (_localizations.ContainsKey(cultureName) && _localizations[cultureName].ContainsKey(name))
        {
            return _localizations[cultureName][name];
        }

        return null;
    }

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        var cultureName = CultureInfo.CurrentCulture.Name.Replace("-", "_");

        if (_localizations.ContainsKey(cultureName))
        {
            foreach (var localization in _localizations[cultureName])
            {
                yield return new LocalizedString(localization.Key, localization.Value, resourceNotFound: false);
            }
        }
    }
}
