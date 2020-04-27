using System;
using Prism.Ioc;
using Prism;
using Template10.Services.Serialization;
using Windows.Storage;

namespace Template10.Services.Settings
{
    public class LocalSettingsAdapter : ISettingsAdapter
    {
        private ApplicationDataContainer _container;

        public LocalSettingsAdapter()
          : this(PrismApplicationBase.Current.Container.Resolve<ISerializationService>())
        {
            // empty
        }

        public LocalSettingsAdapter(ISerializationService serializationService)
        {
            _container = ApplicationData.Current.LocalSettings;
            SerializationService = serializationService;
        }

        public ISerializationService SerializationService { get; }

        public (bool successful, string result) ReadString(string key)
        {
            if (_container.Values.ContainsKey(key))
            {
                return (true, _container.Values[key].ToString());
            }
            else
            {
                return (false, string.Empty);
            }
        }

        public void WriteString(string key, string value)
            => _container.Values[key] = value;
    }
}
