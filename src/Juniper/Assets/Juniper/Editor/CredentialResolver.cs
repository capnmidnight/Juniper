using Juniper.Security;

using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;

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

        public void OnPreprocessBuild(BuildReport report)
        {
            var receivers = Find.All<ICredentialReceiver>();
            foreach (var receiver in receivers)
            {
                receiver.ReceiveCredentials();
            }

            EditorSceneManager.SaveOpenScenes();

            Debug.Log("<== Juniper.CredentialResolver ==>: Credentials set");
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
    }
}