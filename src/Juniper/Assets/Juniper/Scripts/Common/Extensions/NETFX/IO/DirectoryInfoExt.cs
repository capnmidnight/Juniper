using System.Collections.Generic;

namespace System.IO
{
    public static class DirectoryInfoExt
    {
        public static IEnumerable<FileInfo> RecurseFiles(this DirectoryInfo dir)
        {
            var q = new Queue<DirectoryInfo> { dir };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                q.AddRange(here.GetDirectories());
                foreach (var file in here.GetFiles())
                {
                    yield return file;
                }
            }
        }

        public static IEnumerable<DirectoryInfo> RecurseDirectories(this DirectoryInfo dir)
        {
            var q = new Queue<DirectoryInfo> { dir };
            while (q.Count > 0)
            {
                var here = q.Dequeue();
                foreach (var subDir in here.GetDirectories())
                {
                    q.Enqueue(subDir);
                    yield return subDir;
                }
            }
        }

        public static List<string> Nuke(this DirectoryInfo dir)
        {
            var allErrors = new List<string>();
            foreach (var file in dir.RecurseFiles())
            {
                try
                {
                    file.Delete();
                }
                catch
                {
                    allErrors.Add(file.FullName);
                }
            }

            if (allErrors.Count == 0)
            {
                dir.Delete(true);
            }

            return allErrors;
        }
    }
}
