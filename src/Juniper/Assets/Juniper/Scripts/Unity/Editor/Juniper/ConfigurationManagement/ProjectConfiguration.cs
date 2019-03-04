using Newtonsoft.Json;

using System;
using System.IO;

namespace Juniper.ConfigurationManagement
{
    public sealed class ProjectConfiguration
    {
        private const string CONFIG_FILE_NAME = "juniper.json";

        public static ProjectConfiguration Load()
        {
            try
            {
                if (File.Exists(CONFIG_FILE_NAME))
                {
                    return JsonConvert.DeserializeObject<ProjectConfiguration>(File.ReadAllText(CONFIG_FILE_NAME));
                }
            }
            catch
            {
            }

            return new ProjectConfiguration();
        }

        [JsonProperty]
        private int buildStep;

        [JsonProperty]
        private PlatformTypes currentPlatform;

        [JsonProperty]
        private PlatformTypes nextPlatform;

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
            FileExt.WriteAllText(CONFIG_FILE_NAME, JsonConvert.SerializeObject(this));
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public PlatformTypes CurrentPlatform
        {
            get
            {
                return currentPlatform;
            }
        }

        [JsonIgnore]
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
