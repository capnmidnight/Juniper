using System;
using System.IO;
using System.Runtime.Serialization;

using Juniper.IO;
using Juniper.XR;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class ProjectConfiguration : ISerializable
    {
        private const string CONFIG_FILE_NAME = "juniper.json";
        private static readonly JsonFactory<ProjectConfiguration> json = new JsonFactory<ProjectConfiguration>();

        public static ProjectConfiguration Load()
        {
            try
            {
                if (File.Exists(CONFIG_FILE_NAME))
                {
                    return json.Deserialize(CONFIG_FILE_NAME);
                }
            }
            catch
            {
            }

            return new ProjectConfiguration();
        }

        private int buildStep;
        private PlatformType nextPlatform;

        private ProjectConfiguration()
        {

        }

        private ProjectConfiguration(SerializationInfo info, StreamingContext context)
        {
            buildStep = info.GetInt32(nameof(buildStep));
            CurrentPlatform = info.GetEnumFromString<PlatformType>(nameof(CurrentPlatform));
            nextPlatform = info.GetEnumFromString<PlatformType>(nameof(nextPlatform));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(buildStep), buildStep);
            info.AddValue(nameof(CurrentPlatform), CurrentPlatform.ToString());
            info.AddValue(nameof(nextPlatform), nextPlatform.ToString());
        }

        public event Func<bool> PlatformChanged;

        public event Action PlatformChangeConfirmed;

        public void Commit()
        {
            buildStep = 0;
            CurrentPlatform = NextPlatform;
            nextPlatform = PlatformType.None;
            Save();
        }

        private void Save()
        {
            json.Serialize(CONFIG_FILE_NAME, this);
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

        public PlatformType CurrentPlatform { get; private set; }

        public PlatformType NextPlatform
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
                    if (value != PlatformType.None)
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
