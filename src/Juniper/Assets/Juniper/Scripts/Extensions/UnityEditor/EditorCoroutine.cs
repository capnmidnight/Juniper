#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEditor
{
    public sealed class EditorCoroutine
    {
        private readonly Stack<IEnumerator> routines = new Stack<IEnumerator>();

        public EditorCoroutine(IEnumerator root)
        {
            routines.Push(root);
            EditorApplication.update += Update;
        }

        public bool IsComplete
        {
            get
            {
                return routines.Count == 0;
            }
        }

        public bool IsFaulted
        {
            get
            {
                return Error != null;
            }
        }

        public Exception Error
        {
            get;
            private set;
        }

        private void Update()
        {
            try
            {
                if (IsComplete)
                {
                    TearDown();
                }
                else
                {
                    var curRoutine = routines.Pop();
                    if (curRoutine.MoveNext())
                    {
                        routines.Push(curRoutine);

                        var curObj = curRoutine.Current;
                        if (curObj is IEnumerator innerRoutine)
                        {
                            routines.Push(innerRoutine);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                Error = exp;
                TearDown();
            }
        }

        private void TearDown()
        {
            EditorApplication.update -= Update;
        }
    }
}
#endif