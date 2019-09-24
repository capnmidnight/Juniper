using System;
using System.IO;

using Juniper.XR;

using Juniper.Serialization;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class ProjectConfiguration : ISerializable
    {
        private const string CONFIG_FILE_NAME = "juniper.json";
        private static readonly IFactory<ProjectConfiguration> json = new JsonFactory().Specialize<ProjectConfiguration>();

        public static ProjectConfiguration Load()
        {
            try
            {
                if (File.Exists(CONFIG_FILE_NAME))
                {
                    return json.Load(CONFIG_FILE_NAME);
                }
            }
            catch
            {
            }

            return new ProjectConfiguration();
        }

        private int buildStep;
        private PlatformTypes currentPlatform;
        private PlatformTypes nextPlatform;

        private ProjectConfiguration()
        {

        }

        private ProjectConfiguration(SerializationInfo info, StreamingContext context)
        {
            buildStep = info.GetInt32(nameof(buildStep));
            currentPlatform = info.GetEnumFromString<PlatformTypes>(nameof(currentPlatform));
            nextPlatform = info.GetEnumFromString<PlatformTypes>(nameof(nextPlatform));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(buildStep), buildStep);
            info.AddValue(nameof(currentPlatform), currentPlatform.ToString());
            info.AddValue(nameof(nextPlatform), nextPlatform.ToString());
        }

        public event Func<bool> PlatformChanged;

        public event Action PlatformChangeConfirmed;

        public void Commit()
        {
            buildStep = 0;
            currentPlatform = NextPlatform;
            nextPlatform = PlatformTypes.None;
            Save();
        }

        private void Save()
        {
            json.Save(CONFIG_FILE_NAME, this);
        }

        public int BuildStep
        {
            get
            {
                return buildStep - 1;
            }

            set
            {
                buildStep = value + 1;
                Save();
            }
        }

        public PlatformTypes CurrentPlatform
        {
            get
            {
                return currentPlatform;
            }
        }

        public PlatformTypes NextPlatform
        {
            get
            {
                return nextPlatform;
            }

            set
            {
                var lastValue = nextPlatform;
                nextPlatform = value;
                if (PlatformChanged?.Invoke() != false)
                {
                    Save();
                    if (value != PlatformTypes.None)
                    {
                        PlatformChangeConfirmed?.Invoke();
                    }
                }
                else
                {
                    nextPlatform = lastValue;
                }
            }
        }
    }
}
