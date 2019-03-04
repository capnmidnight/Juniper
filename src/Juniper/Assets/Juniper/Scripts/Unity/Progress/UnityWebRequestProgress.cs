#if UNITY_MODULES_UNITYWEBREQUEST

using UnityEngine.Networking;

namespace Juniper.Progress
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