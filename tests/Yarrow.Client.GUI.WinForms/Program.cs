using System;
using System.Threading.Tasks;
using System.Windows.Forms;

using Juniper.Google.Maps;

namespace Yarrow.Client.GUI.WinForms
{
    internal static class Program
    {
        private static ImageViewer form;
        private static YarrowClient yarrow;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.ThreadException += Application_ThreadException;

            yarrow = new YarrowClient();
            form = new ImageViewer();
            form.LocationSubmitted += Form_LocationSubmitted;

            using (form)
            {
                Application.Run(form);
            }
        }

        private static void Form_LocationSubmitted(object sender, string location)
        {
            Task.Run(async () =>
            {
                var metadata = await yarrow.GetMetadata((PlaceName)location);
                var pano = metadata.pano_id;
                var imageFile = await yarrow.GetImage(pano);
                form.SetImage(imageFile);
            });
        }

        private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}