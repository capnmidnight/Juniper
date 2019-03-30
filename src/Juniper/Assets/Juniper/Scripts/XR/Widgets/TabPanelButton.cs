using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Juniper.Unity.Widgets
{
    public class TabPanelButton : MonoBehaviour
    {
        public GameObject panel;

        public void SetActive(bool active)
        {
            var button = GetComponent<Button>();
            button.interactable = !active;
            panel.SetActive(active);
        }

        public void AddListener(UnityAction action)
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(action);
        }
    }
}