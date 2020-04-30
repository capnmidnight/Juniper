using System;
using System.Collections.Generic;
using System.Linq;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Juniper.FreeRDP.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Dictionary<string, (Type type, NavigationViewItem view)> pages;

        public MainPage()
        {
            InitializeComponent();

            var pagesTypes = new Dictionary<string, Type>
            {
                { "Connect", typeof(ConnectionPage) }
            };

            pages = NavView.MenuItems
                .OfType<NavigationViewItem>()
                .ToDictionary(i => (string)i.Tag)
                .ToDictionary(kv => kv.Key, kv => (pagesTypes[kv.Key], kv.Value));
        }

        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            pages.Add("Settings", (typeof(SettingsPage), (NavigationViewItem)NavView.SettingsItem));
            var page = pages.First();
            Navigate(page.Key, page.Value, new EntranceNavigationTransitionInfo());
        }

        private void NavView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            var tag = args.IsSettingsInvoked
                ? "Settings"
                : (string)args.InvokedItemContainer.Tag;
            var page = pages[tag];
            Navigate(tag, page, args.RecommendedNavigationTransitionInfo);
        }

        private void Navigate(string tag, (Type type, NavigationViewItem view) page, NavigationTransitionInfo transitionInfo)
        {
            var prevPage = ContentFrame.CurrentSourcePageType;
            if (page.type != prevPage)
            {
                ContentFrame.Navigate(page.type, tag, transitionInfo);
            }
        }

        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var tag = (string)e.Parameter;
            NavView.IsBackEnabled = ContentFrame.CanGoBack;
            NavView.Header = tag;
            NavView.SelectedItem = pages[tag].view;
        }

        private void NavView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            if (ContentFrame.CanGoBack)
            {
                ContentFrame.GoBack();
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
