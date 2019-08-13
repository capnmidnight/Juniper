using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.Google.Maps.StreetView;

namespace Yarrow.Client
{
    [Serializable]
    public class YarrowEnvironment : ISerializable
    {
        private readonly string title;

        private readonly string description;

        private readonly DateTime createdOn;

        private readonly List<YarrowAudioSource> audioSources;

        private readonly Dictionary<PanoID, YarrowInteractionStation> stations = new Dictionary<PanoID, YarrowInteractionStation>();

        public YarrowEnvironment(string title, string description)
        {
            this.title = title;
            this.description = description;
            createdOn = DateTime.Now;
            audioSources = new List<YarrowAudioSource>();
        }

        protected YarrowEnvironment(SerializationInfo info, StreamingContext context)
        {
            title = info.GetString(nameof(title));
            description = info.GetString(nameof(description));
            audioSources = info.GetList<YarrowAudioSource>(nameof(audioSources));
            var locs = info.GetValue<YarrowInteractionStation[]>(nameof(stations));
            foreach (var station in locs)
            {
                stations[station.Pano] = station;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(title), title);
            info.AddValue(nameof(description), description);
            info.AddList(nameof(audioSources), audioSources);
            info.AddValue(nameof(stations), stations.Values.ToArray());
        }

        public string Title { get { return title; } }

        public string Description { get { return description; } }

        public DateTime CreatedOn { get { return createdOn; } }

        public IEnumerable<YarrowAudioSource> AudioSources { get { return audioSources; } }

        public void AddAudioSource(YarrowAudioSource audioSource)
        {
            audioSources.Add(audioSource);
        }

        public void RemoveAudioSource(YarrowAudioSource audioSource)
        {
            audioSources.Remove(audioSource);
        }

        public IDictionary<PanoID, YarrowInteractionStation> Stations { get { return stations; } }

        public void AddStation(YarrowInteractionStation station)
        {
            stations[station.Pano] = station;
        }

        public void RemoveStation(YarrowInteractionStation station)
        {
            stations.Remove(station.Pano);
        }
    }
}