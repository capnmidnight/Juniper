using System.Collections.Generic;

namespace System.IO
{
    public static class DirectoryExt
    {
        public static IEnumerable<string> RecurseFiles(string path)
        {
            var q = new Queue<string>() { path };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(Directory.GetDirectories(here));
                foreach (var file in Directory.GetFiles(here))
                {
                    yield return file;
                }
            }
        }

        public static IEnumerable<string> RecurseDirectories(string path)
        {
            var q = new Queue<string>() { path };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                foreach (var subDir in Directory.GetDirectories(here))
                {
                    q.Enqueue(subDir);
                    yield return subDir;
                }
            }
        }

        public static bool TryDelete(string path)
        {
            if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static void CreateDirectory(string dir)
        {
            if (!string.IsNullOrWhiteSpace(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }
    }
}
