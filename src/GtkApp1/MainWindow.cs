using System;
using Gtk;
using WebKit;

namespace GtkApp1
{
    class MainWindow : Window
    {
        public readonly WebView webView;

        public MainWindow() : base("MainWindow.glade")
        {
            webView = new WebView();
            Add(webView);
        }
    }
}
