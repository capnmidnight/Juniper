using UnityEngine;
using UnityEngine.Events;

namespace Juniper.Unity
{
    public class EditorTextInput : MonoBehaviour
    {
        [TextArea]
        public string value;

        public StringEvent OnSubmit = new StringEvent();

        public void Submit()
        {
            OnSubmit?.Invoke(value);
        }
    }
}
