using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
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
