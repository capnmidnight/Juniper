using System.Collections.Generic;
using System.Linq;
using Juniper;
using Juniper.Units;
using Juniper.Widgets;
using UnityEngine;

namespace System
{
    /// <summary>
    /// The ScreenDebugger component is a global logging system that has the option of piping its
    /// output to a TextMesh visible on the screen. This can be handy for printing out logging
    /// information on mobile devices without having to explicitly attach a mobile device debugger.
    /// </summary>
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class ScreenDebugger : MonoBehaviour
    {
        /// <summary>
        /// When the ScreenDebugger is instantiated, that instance is saved statically to be
        /// accessible to all code.
        /// </summary>
        public ScreenDebugger()
        {
            instance = this;
        }

        /// <summary>
        /// Clear out the text display
        /// </summary>
        public static void Clear()
        {
            instance?.ClearScreen();
        }

        /// <summary>
        /// Print an exception formatted succinctly for printing to the console. The label field
        /// helps locate the origin of the exception.
        /// </summary>
        /// <param name="exp">  Exp.</param>
        /// <param name="label">Label.</param>
        public static void PrintException(Exception exp, string label)
        {
            Print(exp.ToShortString(label));
        }

        /// <summary>
        /// Print a Vector with components to 3 significant figures.
        /// </summary>
        /// <param name="vector">Vector.</param>
        /// <param name="label"> Label.</param>
        public static void PrintVector(Vector2 vector, string label)
        {
            Print($"[{label}] <{vector.x.Label(UnitOfMeasure.Meters, 3)}, {vector.y.Label(UnitOfMeasure.Meters, 3)}>");
        }

        /// <summary>
        /// Print a Vector with components to 3 significant figures.
        /// </summary>
        /// <param name="vector">Vector.</param>
        /// <param name="label"> Label.</param>
        public static void PrintVector(Vector3 vector, string label)
        {
            Print($"[{label}] <{vector.x.Label(UnitOfMeasure.Meters, 3)}, {vector.y.Label(UnitOfMeasure.Meters, 3)}, {vector.z.Label(UnitOfMeasure.Meters, 3)}>");
        }

        /// <summary>
        /// Print a Vector with components to 3 significant figures.
        /// </summary>
        /// <param name="vector">Vector.</param>
        /// <param name="label"> Label.</param>
        public static void PrintVector(Vector4 vector, string label)
        {
            Print($"[{label}] <{vector.x.Label(UnitOfMeasure.Meters, 3)}, {vector.y.Label(UnitOfMeasure.Meters, 3)}, {vector.z.Label(UnitOfMeasure.Meters, 3)}, {vector.z.Label(UnitOfMeasure.Meters, 4)}>");
        }

        /// <summary>
        /// Print a formatted string
        /// </summary>
        /// <param name="format">Format.</param>
        /// <param name="args">  Arguments.</param>
        public static void PrintFormat(string format, params object[] args)
        {
            Print(string.Format(format, args));
        }

        /// <summary>
        /// Print a simple message
        /// </summary>
        /// <param name="msg">Message.</param>
        public static void Print(string msg)
        {
            if (instance != null && lines != null)
            {
                lines.Add(msg);
            }
            else
            {
                Debug.Log(msg);
            }
        }

        /// <summary>
        /// The text that will be rendered out to the TextMesh at the end of the frame.
        /// </summary>
        private static readonly List<string> lines = new List<string>(10);

        /// <summary>
        /// The screen debugger instance, a singleton.
        /// </summary>
        private static ScreenDebugger instance;

        /// <summary>
        /// The text element to which the output is rendered.
        /// </summary>
        [HideInNormalInspector]
        [SerializeField]
        private TextComponentWrapper text;

        private void Awake()
        {
            GetControls();
            if(text != null)
            {
                ClearScreen();
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            GetControls();
        }
#endif

        private void GetControls()
        {
            if (text == null)
            {
                text = this.Ensure<TextComponentWrapper>();
            }
        }

        /// <summary>
        /// Renders all text in <see cref="lines"/> to the <see cref="text"/> output.
        /// </summary>
        public void LateUpdate()
        {
            if (lines.Count > 0)
            {
                var log = string.Join(Environment.NewLine, lines);
                Debug.Log(log);

                var formatted = from msg in lines
                                select $"{Time.frameCount}: {msg}";
                var output = string.Join(Environment.NewLine, formatted);
                if (text != null)
                {
                    text.text = output;
                }

                lines.Clear();
            }
        }

        /// <summary>
        /// Add lines to the debug output in a way that Unity Events can handle
        /// </summary>
        /// <param name="msg">Message.</param>
        public void AddLine(string msg)
        {
            lines.Add(msg);
            Debug.Log(msg);
        }

        /// <summary>
        /// Remove any text that is in the debugger view right now.
        /// </summary>
        public void ClearScreen()
        {
            lines.Clear();
            text.text = string.Empty;
        }
    }
}
