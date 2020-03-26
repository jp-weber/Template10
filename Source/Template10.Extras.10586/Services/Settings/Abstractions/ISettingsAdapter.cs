using Template10.Services.Serialization;

namespace Template10.Services.Settings
{
    public interface ISettingsAdapter
    {
        (bool successful, string result) ReadString(string key);
        void WriteString(string key, string value);
        ISerializationService SerializationService { get; }
    }
}
