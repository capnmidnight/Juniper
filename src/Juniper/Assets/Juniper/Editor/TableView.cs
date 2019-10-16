using System;
using System.Linq;

using UnityEngine;

namespace UnityEditor
{
    public class TableView
    {
        private readonly (string Header, GUILayoutOption[] Width)[] headers;
        private readonly string title;
        private Vector2 scrollPosition;

        public TableView(string title, params (string Header, GUILayoutOption Width)[] headers)
        {
            this.title = title;
            this.headers = (from h in headers
                            select (h.Header, new GUILayoutOption[] { h.Width }))
                        .ToArray();
        }

        public TableView(params (string Header, GUILayoutOption Width)[] headers)
            : this(null, headers)
        { }

        public TableView(string title, params (string Header, float Width)[] headers)
            : this(title, (from h in headers
                            select (h.Header, GUILayout.Width(h.Width)))
                        .ToArray())
        { }

        public TableView(params (string Header, float Width)[] headers)
            : this(null, headers)
        { }

        public IDisposable Begin()
        {
            if (title != null)
            {
                using (_ = new Header(title))
                {
                    DrawBody();
                }
            }
            else
            {
                DrawBody();
            }
            return new ScrollViewState(ref scrollPosition);
        }

        private void DrawBody()
        {
            using (_ = new HGroup())
            {
                foreach (var header in headers)
                {
                    EditorGUILayout.LabelField(header.Header, EditorStyles.centeredGreyMiniLabel, header.Width);
                }
            }
        }

        private class ScrollViewState : IDisposable
        {
            public ScrollViewState(ref Vector2 scrollPosition)
            {
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, Array.Empty<GUILayoutOption>());
            }

            public void Dispose()
            {
                EditorGUILayout.EndScrollView();
            }
        }
    }
}
