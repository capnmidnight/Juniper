using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Unity.Widgets
{
    public class Selectable : MonoBehaviour, ISelectHandler
    {
        public UnityEvent onSelected;

        public void OnSelect(BaseEventData eventData)
        {
            onSelected?.Invoke();
        }
    }
}