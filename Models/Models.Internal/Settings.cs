using System.Text.Json;

namespace Models.Internal
{
    public static class Settings
    {

        public static Guid? UserId
        {
            get
            {
                var serializedValue = Preferences.Get(nameof(UserId), string.Empty);
                Guid? deserializedValue;
                if (string.IsNullOrEmpty(serializedValue))
                {
                    return null;
                }
                deserializedValue = JsonSerializer.Deserialize<Guid?>(serializedValue);
                return deserializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Set(nameof(UserId), serializedValue);
            }
        }

        public static Guid? DeviceId
        {
            get
            {
                var serializedValue = Preferences.Get(nameof(DeviceId), string.Empty);
                Guid? deserializedValue;
                if (string.IsNullOrEmpty(serializedValue))
                {
                    deserializedValue = Guid.NewGuid();
                    serializedValue = JsonSerializer.Serialize(deserializedValue, new JsonSerializerOptions()
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    Preferences.Set(nameof(DeviceId), serializedValue);
                }
                deserializedValue = JsonSerializer.Deserialize<Guid?>(serializedValue);
                return deserializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Set(nameof(DeviceId), serializedValue);
            }
        }

        public static string RefreshToken
        {
            get
            {
                var serializedValue = Preferences.Get(nameof(RefreshToken), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Set(nameof(RefreshToken), serializedValue);
            }
        }

        public static string UserToken
        {
            get
            {
                var serializedValue = Preferences.Get(nameof(UserToken), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                return deSerializedValue;
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Set(nameof(UserToken), serializedValue);
            }
        }

        public static string EmailAddress
        {
            get
            {
                var serializedValue = Preferences.Get(nameof(EmailAddress), string.Empty);
                if (string.IsNullOrEmpty(serializedValue)) return string.Empty;
                try
                {
                    var deSerializedValue = JsonSerializer.Deserialize<string>(serializedValue);
                    return deSerializedValue;
                }
                catch
                {
                    return string.Empty;
                }
            }
            set
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions()
                {
                    PropertyNameCaseInsensitive = true
                });
                Preferences.Set(nameof(EmailAddress), serializedValue);
            }
        }

    }
}
