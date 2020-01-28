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
                            select (h.Header, h.Width is null ? null : new GUILayoutOption[] { h.Width }))
                        .ToArray();
        }

        public TableView(params (string Header, GUILayoutOption Width)[] headers)
            : this(null, headers)
        { }

        public TableView(string title, params (string Header, float? Width)[] headers)
            : this(title, (from h in headers
                           select (h.Header, h.Width is null ? null : GUILayout.Width(h.Width.Value)))
                        .ToArray())
        { }

        public TableView(params (string Header, float? Width)[] headers)
            : this(null, headers)
        { }

        public IDisposable Begin()
        {
            if (title != null)
            {
                using (new Header(title))
                {
                    DrawHeaders();
                }
            }
            else
            {
                DrawHeaders();
            }

            return new ScrollViewState(ref scrollPosition);
        }

        private void DrawHeaders()
        {
            using (new HGroup())
            {
                foreach (var header in headers)
                {
                    if (header.Width is null)
                    {
                        EditorGUILayout.LabelField(header.Header, EditorStyles.centeredGreyMiniLabel);
                    }
                    else
                    {
                        EditorGUILayout.LabelField(header.Header, EditorStyles.centeredGreyMiniLabel, header.Width);
                    }
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
