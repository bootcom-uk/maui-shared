namespace Interfaces
{
    public interface IDateTimeService
    {

        Task<DateTime?> CurrentDateTime();

        Task<DateTime> FromEpochDateTime(long ticks);

    }
}
