using Juniper.Units;

namespace Juniper.Examples
{
    class Options
    {
        private bool parseLevel = false;
        public Level level = Level.None;

        public Options(string[] args)
        {
            foreach(var arg in args)
            {
                ProcessArg(arg);
            }
        }

        private void ProcessArg(string arg)
        {
            if (parseLevel)
            {
                level = Enum.Parse<Level>(arg);
                parseLevel = false;
            }
            else if (arg == "--level")
            {
                parseLevel = true;
            }
        }
    }
}