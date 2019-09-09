using System.Collections;

using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    public class Tooltipable :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        public Transform tooltip;

        private AbstractStateController trans;

        private bool wasSelected;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (tooltip == null)
            {
                tooltip = transform.Find("Tooltip");
            }
        }
#endif

        public void Awake()
        {
            if (tooltip == null)
            {
                enabled = false;
            }
            else
            {
                trans = tooltip.GetComponent<AbstractStateController>();
            }
        }

        public void OnEnable()
        {
            if (trans != null)
            {
                trans.SkipExit();
            }
            tooltip.Deactivate();
        }

        private void ShowTooltip()
        {
            tooltip.Activate();
            if (trans != null && trans.IsExited)
            {
                trans.Enter();
            }
        }

        private void HideTooltip()
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine(HideTooltipCoroutine());
            }
        }

        private IEnumerator HideTooltipCoroutine()
        {
            if (trans != null && trans.IsEntered)
            {
                yield return trans.ExitCoroutine();
            }
            tooltip.Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CancelInvoke(nameof(HideTooltip));
            ShowTooltip();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Invoke(nameof(HideTooltip), 0.5f);
        }
    }
}
