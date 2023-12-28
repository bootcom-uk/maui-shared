namespace Interfaces
{
    public interface IDataSyncJob
    {

        int NumberOfAttempts { get; set; }

        Task<bool> ExecuteAsync();

        string PreExecutionMessage { get; set; }

        string FailureMessage { get; set; }

        string SuccessMessage { get; set; }


    }
}
