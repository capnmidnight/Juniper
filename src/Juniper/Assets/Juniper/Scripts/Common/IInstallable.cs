using System;
using System.Collections.Generic;

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
        public static void InstallAll(Func<IEnumerable<IInstallable>> getInstallables)
        {
            var found = new List<IInstallable>();
            var keepFinding = true;
            for (var i = 0; i < 10 && keepFinding; ++i)
            {
                keepFinding = false;
                foreach (var installable in getInstallables())
                {
                    if (!found.Contains(installable))
                    {
                        keepFinding = true;
                        if (installable.Install(true))
                        {
                            found.Add(installable);
                        }
                    }
                }
            }
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
