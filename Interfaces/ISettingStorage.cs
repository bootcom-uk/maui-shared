namespace Interfaces
{
    public interface ISettingStorage
    {

        T GetSetting<T>(string key);

        void SetSetting<T>(string key, T settingValue);

    }
}
