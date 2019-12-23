using System;
using System.Collections.Generic;

using Juniper.Progress;

namespace Juniper
{

    /// <summary>
    /// IInstallable utility functions.
    /// </summary>
    public static class Installable
    {
        /// <summary>
        /// Given a callback for finding all modules in a system, runs until either we exhaust
        /// the retry count (default 10) or every installable successfully installs.
        /// </summary>
        /// <param name="getInstallables"></param>
        /// <param name="reset"></param>
        /// <param name="prog">Progress tracker (non by default).</param>
        /// <returns>The number of objects that didn't install correctly.</returns>
        public static List<KeyValuePair<IInstallable, Exception>> InstallAll(Func<List<IInstallable>> getInstallables, bool reset, IProgress prog)
        {
            var errored = new List<KeyValuePair<IInstallable, Exception>>(10);
            var installed = new List<IInstallable>(10);
            var keepFinding = true;
            for (var i = 0; i < 10 && keepFinding; ++i)
            {
                keepFinding = false;

                var installables = getInstallables();
                prog.Report(installed.Count, installables.Count);

                foreach (var installable in installables)
                {
                    if (!installed.Contains(installable))
                    {
                        keepFinding = true;
                        try
                        {
                            for (var j = errored.Count - 1; j >= 0; --j)
                            {
                                if (errored[j].Key == installable)
                                {
                                    errored.RemoveAt(j);
                                }
                            }

                            installable.Install(reset);
                            installed.Add(installable);
                            prog.Report(installed.Count, installables.Count);
                        }
#pragma warning disable CA1031 // Do not catch general exception types
                        catch (Exception exp)
                        {
                            errored.Add(new KeyValuePair<IInstallable, Exception>(installable, exp));
                        }
#pragma warning restore CA1031 // Do not catch general exception types
                    }
                }
            }

            return errored;
        }

        public static List<KeyValuePair<IInstallable, Exception>> InstallAll(Func<List<IInstallable>> getInstallables, bool reset)
        {
            return InstallAll(getInstallables, reset, null);
        }

        /// <summary>
        /// Given a callback for finding all modules in a system, runs the uninstall
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