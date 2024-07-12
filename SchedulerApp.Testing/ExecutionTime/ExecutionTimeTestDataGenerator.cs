namespace SchedulerApp.Testing.ExecutionTime;

public class ExecutionTimeTestData
{
    public DateTime ConfigurationDateTime { get; set; }
    public DateTime CurrentDate { get; set; }

    public ExecutionTimeTestData(DateTime configurationDateTime, DateTime currentDate)
    {
        ConfigurationDateTime = configurationDateTime;
        CurrentDate = currentDate;
    }
}

public class ExecutionTimeTestDataGenerator
{
    public static IEnumerable<object[]> GetExecutionTimeTestData()
    {
        yield return new object[] { new ExecutionTimeTestData(new DateTime(2024, 01, 01, 11, 0, 0), new DateTime(2024, 01, 01, 10, 0, 0)) };
        yield return new object[] { new ExecutionTimeTestData(new DateTime(2024, 01, 01, 16, 0, 0), new DateTime(2024, 01, 01, 15, 59, 59)) };
    }
}

