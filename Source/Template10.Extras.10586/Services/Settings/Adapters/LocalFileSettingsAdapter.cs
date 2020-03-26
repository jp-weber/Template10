using System;
using Template10.Extensions;
using Prism.Ioc;
using Template10.Services.File;
using Template10.Services.Serialization;
using Prism;

namespace Template10.Services.Settings
{
    public class LocalFileSettingsAdapter : ISettingsAdapter
    {
        private readonly IFileService _helper;

        public LocalFileSettingsAdapter()
            : this(PrismApplicationBase.Current.Container.Resolve<ISerializationService>())
        {
            // empty
        }

        public LocalFileSettingsAdapter(ISerializationService serializationService)
        {
            _helper = new File.FileService(serializationService);
            SerializationService = serializationService;
        }

        public ISerializationService SerializationService { get; }

        public LocalFileSettingsAdapter(IFileService fileService)
        {
            _helper = fileService;
        }

        public (bool successful, string result) ReadString(string key)
        {
            try
            {
                return (true, _helper.ReadStringAsync(key).Result);
            }
            catch (Exception exc)
            {

                return (false, exc.Message);
            }
        }

        public void WriteString(string key, string value)
        {
            if (!_helper.WriteStringAsync(key, value).Result)
            {
                throw new Exception();
            }
        }
    }
}
