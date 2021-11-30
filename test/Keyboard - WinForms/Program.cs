using System;
using System.Windows.Forms;


namespace Juniper
{
    public static class Program
    {
        private static KeyInputForm form;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            using var f = form = new KeyInputForm();
            Application.Run(form);
        }
    }
}
