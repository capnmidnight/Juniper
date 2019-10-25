using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

using UnityEditor;

using UnityEngine;

using static UnityEditor.EditorGUILayout;
using static UnityEngine.GUILayout;

namespace Juniper.Unity.Editor
{
    [SuppressMessage("Performance", "HAA0101:Array allocation for params parameter", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0102:Non-overridden virtual method call on value type", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0202:Value type to reference type conversion allocation for string concatenation", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "<Pending>")]
    [SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "<Pending>")]
    public class JuniperEditorWindow : EditorWindow
    {
        private static readonly TableView errorView = new TableView(
            "ERROR",
            ("Stack Trace", 300));

        protected static readonly GUILayoutOption availFieldWidth = Width(50);
        protected static readonly GUILayoutOption shortLabelWidth = Width(75);
        protected static readonly GUILayoutOption distFieldWidth = Width(100);
        protected static readonly GUILayoutOption panoFieldWidth = Width(200);
        protected static readonly GUILayoutOption latLngFieldWidth = Width(250);

        private static bool initialized;
        private static Task watcherTask;

        private CancellationTokenSource tokenSource;
        private CancellationToken cancelToken;
        private TaskFactory mainThread;

        protected Task RepaintAsync()
        {
            return mainThread.StartNew(Repaint);
        }

        private readonly GUIContent windowTitle;

        protected JuniperEditorWindow(string title)
        {
            windowTitle = new GUIContent(title);

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            mainThread = new TaskFactory(scheduler);

            EditorApplication.update += OnEditorUpdate;
            Selection.selectionChanged += OnSelectionChanged;
        }

        protected virtual void OnInit()
        { }

        protected virtual void OnEditorUpdate()
        { }

        protected virtual void OnBackgroundUpdate()
        { }

        protected virtual void OnSelectionChanged()
        { }

        protected virtual void OnPaint()
        { }

        private void OnGUI()
        {
            titleContent = windowTitle;

            if (!initialized)
            {
                initialized = true;
                OnInit();
            }

            if (watcherTask.IsRunning())
            {
                using (_ = new HGroup())
                {
                    LabelField("Watcher task running");
                    if (Button("Stop", Width(75)))
                    {
                        StopWatcher();
                    }
                }
            }
            else
            {
                using (_ = new HGroup())
                {
                    if (watcherTask?.IsCanceled == true)
                    {
                        LabelField("Watcher task canceled");
                    }
                    if (Button("Start", Width(75)))
                    {
                        StartWatcher();
                    }
                }

                if (watcherTask?.IsFaulted == true)
                    {
                        using (_ = errorView.Begin())
                        {
                            LabelField(watcherTask.Exception.Message);
                            LabelField(watcherTask.Exception.StackTrace, EditorStyles.wordWrappedLabel);
                            foreach (var exp in watcherTask.Exception.InnerExceptions)
                            {
                                LabelField(exp.Message);
                                LabelField(exp.StackTrace, EditorStyles.wordWrappedLabel);
                            }
                        }
                }
            }

            OnPaint();

            if (watcherTask == null)
            {
                StartWatcher();
            }
        }

        private void StartWatcher()
        {
            if (tokenSource != null)
            {
                tokenSource.Dispose();
                tokenSource = null;
            }

            tokenSource = new CancellationTokenSource();
            cancelToken = tokenSource.Token;

            watcherTask = Task.Run(BackgroundThread, cancelToken);
            watcherTask.ConfigureAwait(false);
        }

        private void StopWatcher()
        {
            tokenSource.Cancel();
        }

        private void BackgroundThread()
        {
            while (true)
            {
                cancelToken.ThrowIfCancellationRequested();

                OnBackgroundUpdate();
            }
        }
    }
}