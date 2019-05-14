using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;

namespace Juniper.Widgets
{
    public class FileSelector : MonoBehaviour
    {
        public SelectionMode mode;

        public Font font;

        public int fontSize = 20;

        public string filter;

        public string startDirectory;

        public enum SelectionMode
        {
            File,
            Directory
        }

        public string SelectedPath
        {
            get; private set;
        }

        public void OK()
        {
            if (Directory.Exists(SelectedPath))
            {
                startDirectory = SelectedPath;
            }
            else if (File.Exists(SelectedPath))
            {
                startDirectory = Path.GetDirectoryName(SelectedPath);
            }

            var select = onSelected;
            CallAndExit(() =>
                select(SelectedPath));
        }

        public void Cancel()
        {
            CallAndExit(onCancel);
        }

        public void Show(string titleText, SelectionMode m, string filterPattern, Action<string> onFileSelected, Action onSelectionCanceled = null)
        {
            gameObject.SetActive(true);
            title.text = titleText;
            mode = m;
            onSelected = onFileSelected;
            onCancel = onSelectionCanceled;
            filter = filterPattern;
        }

        public void Awake()
        {
            container = GetComponentInChildren<ListView>();

            title = Get<Text>("Title");
            label = Get<Text>("Controls/Label");
            path = Get<InputField>("Controls/Path");
            ok = Get<Button>("Controls/OK");
            cancel = Get<Button>("Controls/Cancel");

            ok.onClick.AddListener(OK);
            cancel.onClick.AddListener(Cancel);
        }

        public void Update()
        {
            if (startDirectory != lastStartDirectory)
            {
                ReadDirectory(startDirectory);
                lastStartDirectory = startDirectory;
            }
        }

        public void MaybeSetStartDirectory(DirectoryInfo top)
        {
            while (top?.Exists == false)
            {
                top = top.Parent;
            }

            if (top == null)
            {
                top = new DirectoryInfo(Directory.GetCurrentDirectory());
            }

            startDirectory = top.FullName;
        }

        private string lastFilter;
        private Regex fileFilter;
        private string lastStartDirectory;
        private ListView container;
        private Action<string> onSelected;
        private Action onCancel;
        private Text title;
        private Text label;
        private InputField path;
        private Button ok, cancel;

        private void CallAndExit(Action cb)
        {
            gameObject.SetActive(false);
            onSelected = null;
            onCancel = null;
            cb?.Invoke();
        }

        private T Get<T>(string p) where T : Behaviour
        {
            var t = transform.Find(p);
            if (t == null)
            {
                return default;
            }
            else
            {
                return t.GetComponent<T>();
            }
        }

        private void ReadDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                container.Clear();

                var dirs = Directory.GetDirectories(directory).ToList();
                string parent = null;
                var parentDir = Directory.GetParent(directory);
                if (parentDir != null)
                {
                    parent = parentDir.FullName;
                    dirs.Insert(0, "..");
                }

                if (mode == SelectionMode.File)
                {
                    ok.interactable = false;
                }

                var dir = new DirectoryInfo(directory);
                ShowPath(dir.FullName);

                if (filter != lastFilter)
                {
                    fileFilter = new Regex(filter.Replace(".", "\\.").Replace("*", ".+"));
                    lastFilter = filter + "$";
                }

                for (var i = 0; i < dirs.Count; ++i)
                {
                    AddEntry(SelectionMode.Directory, dirs[i], parent);
                }

                if (mode == SelectionMode.File)
                {
                    var files = Directory.GetFiles(directory).ToList();
                    var j = 0;
                    for (var i = 0; i < files.Count; ++i)
                    {
                        AddEntry(SelectionMode.File, files[i], directory);
                        ++j;
                    }
                }
            }
        }

        private bool FilePassesFilter(string filePath)
        {
            return string.IsNullOrEmpty(filter)
                || fileFilter.IsMatch(filePath);
        }

        private void ShowPath(string fileOrDirPath)
        {
            SelectedPath = fileOrDirPath;
            if (string.IsNullOrEmpty(filter))
            {
                label.text = $"{mode}:";
            }
            else
            {
                label.text = $"{mode} ({filter}):";
            }

            path.text = SelectedPath;
        }

        private void AddEntry(SelectionMode type, string entry, string parent)
        {
            string itemTitle = null;
            string itemKey = null;
            object data = null;
            if (type == SelectionMode.File)
            {
                var file = new FileInfo(entry);
                data = file;
                itemTitle = file.Name;
                itemKey = file.FullName;
            }
            else if (entry == "..")
            {
                var dir = new DirectoryInfo(parent);
                data = dir;
                itemTitle = "../";
                itemKey = dir.FullName;
            }
            else
            {
                var dir = new DirectoryInfo(entry);
                data = dir;
                itemTitle = $" + {dir.Name}/";
                itemKey = dir.FullName;
            }

            var item = container.AddItem(itemTitle, data, itemKey);
            item.AddListener(() =>
                Select(item));
            item.interactable = type == SelectionMode.Directory || FilePassesFilter(entry);
        }

        private void Select(ListViewItem item)
        {
            var obj = item.DataItem;
            if (obj is FileInfo file)
            {
                ok.interactable = true;
                ShowPath(file.FullName);
            }
            else if (obj is DirectoryInfo dir)
            {
                ReadDirectory(dir.FullName);
            }
        }
    }
}
