using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Juniper.Collections
{
    /// <summary>
    /// A representation of files in a tree structure by directory.
    /// </summary>
    public class FileTree : NAryTree<string, FileTree>
    {
        public readonly bool IsDirectory;

        private FileTree(string value, bool isDirectory)
            : base(value)
        {
            IsDirectory = isDirectory;
        }

        public FileTree(string root) : this(root, true)
        {
        }

        public void AddPath(string path, bool isDirectory)
        {
            path = PathExt.FixPath(path);
            var parts = path.Split(Path.DirectorySeparatorChar);
            var q = new Queue<string>(parts);
            var parent = q.Dequeue();
            if (parent == Value)
            {
                var here = this;
                while (q.Count > 0 && here != null)
                {
                    var pathPart = q.Dequeue();
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
                        next = new FileTree(pathPart, q.Count > 0 || isDirectory);
                        here.Add(next);
                    }

                    here = next;
                }
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
            var q = new Queue<FileTree>
            {
                this
            };
            while (q.Count > 0)
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

            while (stack.Count > 0)
            {
                var here = stack.Pop();
                var path = Path.Combine(root, here);
                File.Delete(path);
            }
        }
    }
}