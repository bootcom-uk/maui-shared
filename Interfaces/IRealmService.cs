using Realms;

namespace Interfaces
{
    public interface IRealmService
    {

        Realm Realm { get; }

        Task InitializeAsync();

    }
}
