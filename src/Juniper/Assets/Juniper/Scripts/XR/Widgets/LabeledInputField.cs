using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Juniper.Widgets
{
    public class LabeledInputField : MonoBehaviour
    {
        public StringEvent onValueChanged = new StringEvent();
        public UnityEvent onSelected = new UnityEvent();

        public Text Label
        {
            get; private set;
        }

        public InputField Field
        {
            get; private set;
        }

        public string LabelText
        {
            get
            {
                return Label.text;
            }
            set
            {
                if (Label.text != value)
                {
                    Label.text = value;
                }
            }
        }

        public string Text
        {
            get
            {
                return Field.text;
            }
            set
            {
                if (Field.text != value)
                {
                    Field.text = value;
                }
            }
        }

        public bool Interactable
        {
            get
            {
                return Field.interactable;
            }
            set
            {
                Field.interactable = value;
                HighlightLabel(Indicated);
            }
        }

        public bool Indicated
        {
            get
            {
                return indicatedBacker;
            }
            set
            {
                HighlightLabel(indicatedBacker = value);
            }
        }

        public void Awake()
        {
            Label = GetComponentInChildren<Text>();
            Field = GetComponentInChildren<InputField>();
            Field.onValueChanged.AddListener(onValueChanged.Invoke);
            Field.Ensure<Selectable>()
                .Value
                .onSelected
                .AddListener(onSelected.Invoke);
        }

        private bool indicatedBacker;

        private void HighlightLabel(bool value)
        {
            if (!Interactable)
            {
                Label.color = Color.grey;
                Label.fontStyle = FontStyle.Normal;
            }
            else if (value)
            {
                Label.color = Color.red;
                Label.fontStyle = FontStyle.Italic;
            }
            else
            {
                Label.color = Color.black;
                Label.fontStyle = FontStyle.Normal;
            }
        }
    }
}
