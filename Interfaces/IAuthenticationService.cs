namespace Interfaces
{
    public interface IAuthenticationService<IdType>
    {

        Task<IdType> CollectUserId();

        Task<string> ValidateEmailToken(Guid emailToken);

        Task<Dictionary<string, string>> ValidateQuickAccessCode(string quickAccessCode);

        Task<bool> RequestLogin(string emailAddress, string uriScheme);

    }
}
