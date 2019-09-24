using System.Linq;

using Juniper.Security;

using UnityEditor.Build;
using UnityEditor.Build.Reporting;

using UnityEngine;

namespace Juniper
{
    public class CredentialResolver :
        IPreprocessBuildWithReport, IPostprocessBuildWithReport
    {
        public int callbackOrder
        {
            get
            {
                return 0;
            }
        }

        public void OnPostprocessBuild(BuildReport report)
        {
            var receivers = Find.All<ICredentialReceiver>();
            foreach (var receiver in receivers)
            {
                receiver.ClearCredentials();
            }
            Debug.Log("<== Juniper.CredentialResolver ==>: Credentials cleared");
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            var receivers = Find.All<ICredentialReceiver>();
            foreach (var receiver in receivers)
            {
                receiver.ReceiveCredentials();
            }
            Debug.Log("<== Juniper.CredentialResolver ==>: Credentials set");
        }
    }
}