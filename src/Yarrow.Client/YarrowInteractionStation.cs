using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Juniper.Google.Maps.StreetView;

namespace Yarrow.Client
{
    [Serializable]
    public class YarrowInteractionStation : ISerializable
    {
        private readonly string title;

        private readonly PanoID pano_id;

        private readonly List<PanoID> exits;

        private readonly List<YarrowAudioSource> audioSources;

        private readonly List<YarrowInteractionPoint> interactionPoints;

        private readonly List<YarrowStaticImages> staticImages;

        public YarrowInteractionStation(PanoID pano_id, string title)
        {
            this.pano_id = pano_id;
            this.title = title;
            exits = new List<PanoID>();
            audioSources = new List<YarrowAudioSource>();
            interactionPoints = new List<YarrowInteractionPoint>();
            staticImages = new List<YarrowStaticImages>();
        }

        protected YarrowInteractionStation(SerializationInfo info, StreamingContext context)
        {
            pano_id = (PanoID)info.GetString(nameof(pano_id));
            title = info.GetString(nameof(title));

            exits = info.GetList<PanoID>(nameof(exits));
            audioSources = info.GetList<YarrowAudioSource>(nameof(audioSources));
            interactionPoints = info.GetList<YarrowInteractionPoint>(nameof(interactionPoints));
            staticImages = info.GetList<YarrowStaticImages>(nameof(staticImages));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(pano_id), pano_id.ToString());
            info.AddValue(nameof(title), title);
            info.AddList(nameof(exits), exits);
            info.AddList(nameof(audioSources), audioSources);
            info.AddList(nameof(interactionPoints), interactionPoints);
            info.AddList(nameof(staticImages), staticImages);
        }

        public PanoID Pano { get { return pano_id; } }

        public string Title { get { return title; } }

        public void AddExit(PanoID pano_id)
        {
            exits.Add(pano_id);
        }

        public void RemoveExit(PanoID pano_id)
        {
            exits.Remove(pano_id);
        }

        public IEnumerable<YarrowAudioSource> AudioSources { get { return audioSources; } }

        public void AddAudioSource(YarrowAudioSource audioSource)
        {
            audioSources.Add(audioSource);
        }

        public void RemoveAudioSource(YarrowAudioSource audioSource)
        {
            audioSources.Remove(audioSource);
        }

        public IEnumerable<YarrowInteractionPoint> InteractionPoints { get { return interactionPoints; } }

        public void AddInteractionPoint(YarrowInteractionPoint interactionPoint)
        {
            interactionPoints.Add(interactionPoint);
        }

        public void RemoveInteractionPoint(YarrowInteractionPoint interactionPoint)
        {
            interactionPoints.Remove(interactionPoint);
        }

        public IEnumerable<YarrowStaticImages> StaticImages { get { return staticImages; } }

        public void AddStaticImage(YarrowStaticImages staticImage)
        {
            staticImages.Add(staticImage);
        }

        public void RemoveStaticImage(YarrowStaticImages staticImage)
        {
            staticImages.Remove(staticImage);
        }
    }
}