using System;
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

        private readonly GUIContent windowTitle;
        private readonly TaskFactory mainThread;
        private readonly bool startWatcher;

        private CancellationTokenSource tokenSource;
        private CancellationToken cancelToken;
        private Task watcherTask;

        private Exception CurrentError;
        private bool initialized;

        protected Task RepaintAsync()
        {
            return mainThread.StartNew(Repaint);
        }

        protected JuniperEditorWindow(string title, bool startWatcher)
        {
            windowTitle = new GUIContent(title);
            this.startWatcher = startWatcher;

            var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
            mainThread = new TaskFactory(scheduler);

            EditorApplication.update += OnEditorUpdateInternal;
            Selection.selectionChanged += OnSelectionChangedInternal;
        }

        protected virtual void OnEditorUpdate() { }
        private void OnEditorUpdateInternal()
        {
            try
            {
                if (!JuniperSystem.IsMainThreadReady)
                {
                    JuniperSystem.CreateFactory();
                }
                OnEditorUpdate();
            }
            catch (Exception exp)
            {
                CurrentError = new Exception("Error occured during editor update", exp);
            }
        }

        protected virtual void OnBackgroundUpdate() { }
        private void OnBackgroundUpdateInternal()
        {
            try
            {
                if (JuniperSystem.IsMainThreadReady)
                {
                    OnBackgroundUpdate();
                }
            }
            catch (Exception exp)
            {
                CurrentError = new Exception("Error occured during background update", exp);
            }
        }

        protected virtual void OnSelectionChanged() { }
        private void OnSelectionChangedInternal()
        {
            try
            {
                OnSelectionChanged();
            }
            catch (Exception exp)
            {
                CurrentError = new Exception("Error occured during selection change", exp);
            }
        }

        protected virtual void OnPaint()
        { }
        private void OnPaintInternal()
        {
            try
            {
                OnPaint();
            }
            catch (Exception exp)
            {
                CurrentError = new Exception("Error occured during repaint", exp);
            }
        }

        private void OnGUI()
        {
            titleContent = windowTitle;

            if (!startWatcher)
            {
                Init();
            }
            else
            {
                if (watcherTask.IsRunning())
                {
                    using (new HGroup())
                    {
                        LabelField("Watcher task running");
                        Button("Stop", StopWatcher, Width(75));
                    }
                }
                else if (watcherTask != null)
                {
                    using (new HGroup())
                    {
                        if (watcherTask.IsCanceled == true)
                        {
                            LabelField("Watcher task canceled");
                        }

                        Button("Start", StartWatcher, Width(75));
                    }

                    if (watcherTask.IsFaulted == true
                        && CurrentError == null)
                    {
                        CurrentError = new Exception("Error occured on background task", watcherTask.Exception);
                        initialized = false;
                    }
                }
            }

            if (CurrentError != null)
            {
                Button("Clear error", ClearError);

                using (errorView.Begin())
                {
                    var head = CurrentError;
                    while (head != null)
                    {
                        LabelField(head.Message);
                        LabelField(head.StackTrace, EditorStyles.wordWrappedLabel);
                        head = head.InnerException;
                    }
                }
            }

            OnPaintInternal();

            if (startWatcher
                && watcherTask == null)
            {
                StartWatcher();
            }
        }

        protected bool Button(bool enabled, string title, Action act, params GUILayoutOption[] options)
        {
            using (new EditorGUI.DisabledScope(!enabled))
            {
                return Button(title, act, options);
            }
        }

        protected bool Button(string title, Action act, params GUILayoutOption[] options)
        {
            if(GUILayout.Button(title, options))
            {
                act();
                return true;
            }

            return false;
        }

        protected bool Button(string title, params GUILayoutOption[] options)
        {
            return GUILayout.Button(title, options);
        }

        private void Init()
        {
            ClearError();
            OnSelectionChangedInternal();
        }

        private void ClearError()
        {
            CurrentError = null;
        }

        private void StartWatcher()
        {
            Init();

            try
            {
                if (tokenSource != null)
                {
                    tokenSource.Dispose();
                    tokenSource = null;
                }

                tokenSource = new CancellationTokenSource();
                cancelToken = tokenSource.Token;

                watcherTask = Task.Run(BackgroundThread, cancelToken);
            }
            catch (Exception exp)
            {
                CurrentError = new Exception("Error occured while starting watcher", exp);
            }
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

                OnBackgroundUpdateInternal();
            }
        }
    }
}