#if UNITY_MODULES_UNITYWEBREQUEST

using Juniper.Progress;

using UnityEngine.Networking;

namespace Juniper.Unity.Progress
{
    public class UnityWebRequestProgress : IProgress
    {
        private readonly UnityWebRequest request;

        public UnityWebRequestProgress(UnityWebRequest request)
        {
            this.request = request;
        }

        public float Progress
        {
            get
            {
                return request.downloadProgress;
            }
        }
    }
}

#endif