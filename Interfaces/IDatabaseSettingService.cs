namespace Interfaces
{
    public interface IDatabaseSettingService
    {

        Task<settingTypeValue> GetSetting<settingTypeValue>(string settingName);

        Task SetSetting<settingTypeValue>(string settingName, settingTypeValue settingValue);

    }
}
