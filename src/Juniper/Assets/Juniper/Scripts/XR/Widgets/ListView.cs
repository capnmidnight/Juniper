using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Juniper.Unity.Widgets
{
    public class ListView : MonoBehaviour
    {
        public SelectionMode selectionMode = SelectionMode.Single;
        public Font font;
        public int fontSize = 24;
        public Color normalBackgroundColor = Color.grey;
        public Color highlightBackgroundColor = Color.Lerp(Color.white, Color.grey, 0.5f);
        public Color normalColor = Color.white;
        public Color disabledColor = Color.grey;
        public Color highlightColor = Color.black;
        public UnityEvent onSelectionChanged;

        public enum SelectionMode
        {
            None,
            Single,
            SingleToggle,
            Multiple
        }

        public List<ListViewItem> Items { get; } = new List<ListViewItem>();
        public List<ListViewItem> Selection { get; } = new List<ListViewItem>();

        public ListViewItem FirstSelectedItem
        {
            get
            {
                return Selection.FirstOrDefault();
            }
        }

        public void Awake()
        {
            GetComponentsInChildren(true, Items);
        }

        public void Clear()
        {
            foreach (var item in Items)
            {
                item.gameObject.Destroy();
            }
            Items.Clear();
            Selection.Clear();
            count = 0;
        }

        public ListViewItem GetItem(string key)
        {
            return Items.FirstOrDefault(i => i.Key == key);
        }

        public bool ContainsItem(string key)
        {
            return GetItem(key) != null;
        }

        public void AddItems<T>(IEnumerable<T> items, Func<T, string> getKey, Func<T, string> getText)
        {
            foreach (var item in items)
            {
                AddItem(getText(item), item, getKey(item));
            }
        }

        public ListViewItem AddItem(string text, object dataItem, string key)
        {
            var item = new GameObject().AddComponent<ListViewItem>();
            item.transform.SetParent(Container.content, false);
            Items.Add(item);

            item.Key = key;
            item.Text = text;
            item.DataItem = dataItem;
            item.normalColor = normalColor;
            item.disabledColor = disabledColor;
            item.BackgroundColor = normalBackgroundColor;
            item.Font = font;
            item.FontSize = fontSize;

            item.RemoveAllListeners();
            item.AddListener(() =>
                ChangeSelection(item));

            item.transform.SetSiblingIndex(count++);

            return item;
        }

        public void ClearSelection()
        {
            SetSelection(EMPTY_SELECTION);
        }

        public void SetSelection(IEnumerable<string> keys)
        {
            SetSelection(keys.Select(GetItem).ToList());
        }

        public void SetSelection(string key)
        {
            SetSelection(new[] { GetItem(key) }.ToList());
        }

        public void RemoveItem(string key)
        {
            var item = GetItem(key);

            if (item != null)
            {
                Items.Remove(item);
                item.gameObject.Destroy();
                --count;
            }
        }

        private static readonly List<ListViewItem> EMPTY_SELECTION = new List<ListViewItem>();
        private int count;

        private ScrollRect Container
        {
            get
            {
                return GetComponent<ScrollRect>();
            }
        }

        private void ChangeSelection(ListViewItem selected)
        {
            var wasInSelection = Selection.Contains(selected);
            var newSelection = Selection.ToList();

            if (selectionMode != SelectionMode.Multiple)
            {
                newSelection.Clear();
            }
            else if (wasInSelection)
            {
                newSelection.Remove(selected);
            }

            if (selectionMode == SelectionMode.Single
                || (!wasInSelection && selectionMode != SelectionMode.None))
            {
                newSelection.Add(selected);
            }

            SetSelection(newSelection);
        }

        private void SetSelection(List<ListViewItem> newSelection)
        {
            var changed = newSelection.Count != Selection.Count
                || !newSelection.All(Selection.Contains);

            if (changed)
            {
                Selection.Clear();
                Selection.AddRange(newSelection);
                foreach (var item in Items)
                {
                    var highlight = Selection.Contains(item);
                    item.BackgroundColor = highlight ? highlightBackgroundColor : normalBackgroundColor;
                    item.normalColor = highlight ? highlightColor : normalColor;
                }
                onSelectionChanged?.Invoke();
            }
        }
    }
}