using System;
using System.Collections;
using System.Threading.Tasks;
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

        [SerializeField]
        [HideInNormalInspector]
        private TextComponentWrapper textElement;

        [SerializeField]
        [HideInNormalInspector]
        private Speakable speech;

        public float delayBeforeDisplay = 1;
        public float delayBeforeHide = 0.5f;

        private AbstractStateController trans;

        private Func<bool> isParentEnabled;

        public bool IsInteractable()
        {
            return enabled && isParentEnabled();
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            GetControls();
        }
#endif

        private void GetControls()
        {
            if (tooltip == null)
            {
                tooltip = transform.Find("Tooltip");
            }

            if (tooltip != null && textElement == null)
            {
                textElement = tooltip.Ensure<TextComponentWrapper>();
                textElement.SetupControls();
            }

            if (speech == null)
            {
                speech = GetComponent<Speakable>();
            }
        }

        public string Text
        {
            get
            {
                GetControls();

                if(textElement != null)
                {
                    return textElement.Text;
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                GetControls();

                if(textElement != null)
                {
                    textElement.Text = value;
                }
            }
        }

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

                if (speech != null)
                {
#if AZURE_SPEECHSDK
                    speech.Play();
#endif
                }
            }
        }

        private void HideTooltip()
        {
            if (isActiveAndEnabled)
            {
                this.Run(HideTooltipCoroutine());
            }
        }

        private IEnumerator HideTooltipCoroutine()
        {
            if (trans != null && trans.CanExit)
            {
                yield return trans.ExitAsync().AsCoroutine();
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
