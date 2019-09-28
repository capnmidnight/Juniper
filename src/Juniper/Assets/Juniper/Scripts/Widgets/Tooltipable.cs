using System;
using System.Collections;
using Juniper.Speech;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Juniper.Widgets
{
    public class Tooltipable :
        MonoBehaviour,
        IPointerEnterHandler,
        IPointerExitHandler
    {
        /// <summary>
        /// A default function for when the Tooltipable is applied to something that
        /// does not have a parent control.
        /// </summary>
        /// <returns></returns>
        private static bool AlwaysEnabled()
        {
            return true;
        }

        public Transform tooltip;

        public float delayBeforeDisplay = 1;
        public float delayBeforeHide = 0.5f;

        private AbstractStateController trans;

        private bool wasSelected;

        private Func<bool> isParentEnabled;

        [SerializeField]
        [HideInInspector]
        private SpeechOutput speech;

        public bool IsInteractable()
        {
            return enabled && isParentEnabled();

        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (tooltip == null)
            {
                tooltip = transform.Find("Tooltip");
            }

            speech = GetComponent<SpeechOutput>();
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

            var keyword = GetComponent<Keywordable>();
            if (keyword != null)
            {
                isParentEnabled = keyword.IsInteractable;
            }
            else
            {
                var parent = GetComponent<IPointerClickHandler>();
                if (parent is UnityEngine.UI.Selectable selectable)
                {
                    isParentEnabled = selectable.IsInteractable;
                }
                else if (parent is AbstractTouchable touchable)
                {
                    isParentEnabled = touchable.IsInteractable;
                }
                else
                {
                    isParentEnabled = AlwaysEnabled;
                }
            }

            Hide();
        }

        public void OnEnable()
        {
            Hide();
        }

        private void Hide()
        {
            if (trans != null)
            {
                trans.SkipExit();
            }

            if (tooltip != null)
            {
                tooltip.Deactivate();
            }
        }

        private void ShowTooltip()
        {
            if (isParentEnabled())
            {
                tooltip.Activate();
                if (trans != null && trans.CanEnter)
                {
                    trans.Enter();
                }

                if(speech != null)
                {
                    var textElement = tooltip.GetComponent<TextComponentWrapper>();
                    if (textElement != null)
                    {
                        speech.Speak(textElement.text);
                    }
                }
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
            if (trans != null && trans.CanExit)
            {
                yield return trans.ExitCoroutine();
            }
            tooltip.Deactivate();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CancelInvoke(nameof(ShowTooltip));
            CancelInvoke(nameof(HideTooltip));
            Invoke(nameof(ShowTooltip), delayBeforeDisplay);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CancelInvoke(nameof(ShowTooltip));
            CancelInvoke(nameof(HideTooltip));
            Invoke(nameof(HideTooltip), delayBeforeHide);
        }
    }
}
