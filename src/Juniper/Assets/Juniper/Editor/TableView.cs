using System;
using System.Linq;

using UnityEngine;

namespace UnityEditor
{
    public class TableView
    {
        private readonly (string Header, GUILayoutOption Width)[] headers;
        private readonly string title;
        private Vector2 scrollPosition;

        public TableView(string title, params (string Header, float Width)[] headers)
        {
            this.title = title;
            this.headers = (from h in headers
                            select (h.Header, GUILayout.Width(h.Width)))
                        .ToArray();
        }

        public IDisposable Begin()
        {
            using(_ = new Header(title))
            using (_ = new HGroup())
            {
                foreach (var header in headers)
                {
                    EditorGUILayout.LabelField(header.Header, EditorStyles.centeredGreyMiniLabel, header.Width);
                }
            }
            return new ScrollViewState(ref scrollPosition);
        }

        private class ScrollViewState : IDisposable
        {
            public ScrollViewState(ref Vector2 scrollPosition)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            }

            public void Dispose()
            {
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
