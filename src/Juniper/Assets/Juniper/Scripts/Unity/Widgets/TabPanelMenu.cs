using System.Linq;

using UnityEngine;

namespace Juniper.Unity.Widgets
{
    public class TabPanelMenu : MonoBehaviour
    {
        public void Awake()
        {
            tabButtons = GetComponentsInChildren<TabPanelButton>();

            foreach (var tabButton in tabButtons)
            {
                tabButton.SetActive(true);
                tabButton.AddListener(() =>
                    ActivateButton(tabButton));
            }

            if (tabButtons.Length > 0)
            {
                ActivateButton(tabButtons.First());
            }
        }

        public void Show(string name)
        {
            var child = transform.Find(name);
            if (child != null)
            {
                ActivateButton(child.GetComponent<TabPanelButton>());
            }
        }

        private TabPanelButton[] tabButtons;

        private void ActivateButton(TabPanelButton clickedButton)
        {
            foreach (var tabButton in tabButtons)
            {
                tabButton.SetActive(tabButton == clickedButton);
            }
        }
    }
}
