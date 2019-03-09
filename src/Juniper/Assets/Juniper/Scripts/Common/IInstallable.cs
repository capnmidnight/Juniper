using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    public interface IInstallable
    {
        bool Install(bool reset);

        void Reinstall();

        void Uninstall();
    }

    public static class Installable
    {
        public static int InstallAll(Func<IEnumerable<IInstallable>> getInstallables)
        {
            var installed = new List<IInstallable>();
            var keepFinding = true;
            for (var i = 0; i < 10 && keepFinding; ++i)
            {
                keepFinding = false;
                foreach (var installable in getInstallables())
                {
                    if (!installed.Contains(installable))
                    {
                        keepFinding = true;
                        if (installable.Install(true))
                        {
                            installed.Add(installable);
                        }
                    }
                }
            }

            return getInstallables().Count() - installed.Count;
        }

        public static void UninstallAll(Func<IEnumerable<IInstallable>> getInstallables)
        {
            foreach (var installable in getInstallables())
            {
                installable.Uninstall();
            }
        }
    }
}
