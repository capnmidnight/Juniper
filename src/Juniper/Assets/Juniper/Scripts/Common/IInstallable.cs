using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper
{
    /// <summary>
    /// Defines an interface for objects that can be installed and have their
    /// installation progress tracked.
    /// </summary>
    public interface IInstallable
    {
        /// <summary>
        /// Install, with or without a full reset.
        /// </summary>
        /// <param name="reset"></param>
        /// <returns>True, if the installation completed successfuly. False if the installable should be
        /// recycled and tried again.</returns>
        bool Install(bool reset);

        /// <summary>
        /// Reinstall the object with a full reset.
        /// </summary>
        void Reinstall();

        /// <summary>
        /// Remove platform-specific features from the object.
        /// </summary>
        void Uninstall();
    }

    /// <summary>
    /// IInstallable utility functions.
    /// </summary>
    public static class Installable
    {
        /// <summary>
        /// Given a callback for finding all installables in a system, runs until either we exhaust
        /// the retry count (default 10) or every installable successfully installs.
        /// </summary>
        /// <param name="getInstallables"></param>
        /// <param name="reset"></param>
        /// <returns></returns>
        public static int InstallAll(Func<IEnumerable<IInstallable>> getInstallables, bool reset)
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
                        if (installable.Install(reset))
                        {
                            installed.Add(installable);
                        }
                    }
                }
            }

            return getInstallables().Count() - installed.Count;
        }

        /// <summary>
        /// Given a callback for finding all installables in a system, runs the uninstall
        /// process for all of them.
        /// </summary>
        /// <param name="getInstallables"></param>
        public static void UninstallAll(Func<IEnumerable<IInstallable>> getInstallables)
        {
            foreach (var installable in getInstallables())
            {
                installable.Uninstall();
            }
        }
    }
}
