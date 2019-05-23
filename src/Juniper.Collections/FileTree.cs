using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Juniper.Collections
{
    /// <summary>
    /// A representation of files in a tree structure by directory.
    /// </summary>
    public class FileTree : NAryTree<string>
    {
        public readonly bool IsDirectory;

        private FileTree(string value, bool isDirectory)
            : base(value)
        {
            IsDirectory = isDirectory;
        }

        public FileTree() : this("", true) { }

        public void AddPath(string path, bool isDirectory)
        {
            path = PathExt.FixPath(path);
            var parts = new Queue<string>(path.Split(Path.DirectorySeparatorChar));
            var here = this;
            while (parts.Count > 0 && here != null)
            {
                var pathPart = parts.Dequeue();
                FileTree next = null;
                foreach (var child in here.children)
                {
                    if (child.Value == pathPart)
                    {
                        next = (FileTree)child;
                        break;
                    }
                }

                if (next == null)
                {
                    next = new FileTree(pathPart, parts.Count > 0 || isDirectory);
                    Add(next);
                }

                here = next;
            }
        }

        public string PathName
        {
            get
            {
                var sb = new StringBuilder();
                var here = this;
                while (here != null)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, Path.DirectorySeparatorChar);
                    }
                    sb.Insert(0, here.Value);
                    here = (FileTree)here.parent;
                }
                return sb.ToString();
            }
        }

        public void Delete(string root)
        {
            var stack = new Stack<string>();
            var q = new Queue<FileTree>();
            q.Add(this);
            while(q.Count > 0)
            {
                var here = q.Dequeue();
                if (!here.IsDirectory)
                {
                    stack.Push(here.PathName);
                }
                else
                {
                    q.AddRange(here.children.Cast<FileTree>());
                }
            }

            while(stack.Count > 0)
            {
                var here = stack.Pop();
                var path = Path.Combine(root, here);
                File.Delete(path);
            }
        }
    }
}
