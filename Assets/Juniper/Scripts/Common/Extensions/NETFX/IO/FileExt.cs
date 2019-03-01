namespace System.IO
{
    public static class FileExt
    {
        public static bool TryDelete(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                    return true;
                }
                catch
                {
                }
            }
            return false;
        }

        public static void Copy(string src, string dest, bool overwrite)
        {
            var destDir = Path.GetDirectoryName(dest);
            DirectoryExt.CreateDirectory(destDir);
            File.Copy(src, dest, overwrite);
        }

        public static void WriteAllText(string path, string text)
        {
            var dir = Path.GetDirectoryName(path);
            DirectoryExt.CreateDirectory(dir);
            File.WriteAllText(path, text);
        }
    }
}
