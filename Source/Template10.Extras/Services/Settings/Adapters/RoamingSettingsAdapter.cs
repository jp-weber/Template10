using System;
using Template10.Services.Serialization;
using Prism.Ioc;
using Windows.Storage;
using Prism;

namespace Template10.Services.Settings
{
    public class RoamingSettingsAdapter : ISettingsAdapter
    {
        private ApplicationDataContainer _container;

        public RoamingSettingsAdapter()
           : this(PrismApplicationBase.Current.Container.Resolve<ISerializationService>())
        {
            // empty
        }

        public RoamingSettingsAdapter(ISerializationService serializationService)
        {
            _container = ApplicationData.Current.RoamingSettings;
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
