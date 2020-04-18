using System;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using win = Windows;
using Prism.Services;
using Windows.UI.Xaml.Controls;
using NavigationView = Microsoft.UI.Xaml.Controls.NavigationView;
using NavigationViewItem = Microsoft.UI.Xaml.Controls.NavigationViewItem;
using Template10.Navigation;
using System.Threading.Tasks;

namespace Template10.Controls
{
    public class NavViewEx : NavigationView
    {
        private CoreDispatcher _dispatcher;
        private Frame _frame;
        public INavigationService NavigationService { get; private set; }

        public NavViewEx()
        {
            DefaultStyleKey = typeof(NavigationView);

            if (win.ApplicationModel.DesignMode.DesignModeEnabled
                || win.ApplicationModel.DesignMode.DesignMode2Enabled)
            {
                return;
            }

            Content = _frame = new Frame();
            _dispatcher = _frame.Dispatcher;

            _frame.Navigated += (s, e) =>
            {
                if (TryFindItem(e.SourcePageType, e.Parameter, out var item))
                {
                    SetSelectedItem(item, false);
                    if (item == null)
                    {
                        return;
                    }
                }
                if (IsPaneOpen && (PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact ||
                PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftMinimal))
                {
                    //await Task.Delay(450);
                    IsPaneOpen = false;
                }
            };

            NavigationService = NavigationFactory.Create(_frame).AttachGestures(Window.Current, Gesture.Back, Gesture.Forward, Gesture.Refresh);

            ItemInvoked += (s, e) =>
            {
                SelectedItem = (e.IsSettingsInvoked) ? SettingsItem : Find(e.InvokedItemContainer as NavigationViewItem);
                if (IsPaneOpen && (PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact ||
                PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftMinimal))
                {
                    //await Task.Delay(450);
                    IsPaneOpen = false;
                }
            };
        }
        public string SettingsNavigationUri { get; set; }
        public event EventHandler SettingsInvoked;

        private object PreviousItem
        {
            get;set;
        }

        public new object SelectedItem
        {
            set => SetSelectedItem(value);
            get => base.SelectedItem;
        }

        private async void SetSelectedItem(object selectedItem, bool withNavigation = true)
        {
            if (selectedItem == null)
            {
                base.SelectedItem = null;
            }
            else if (selectedItem == PreviousItem)
            {
                // already set
            }
            else if (selectedItem == SettingsItem)
            {
                if (SettingsNavigationUri != null)
                {
                    if (withNavigation)
                    {
                        if ((await NavigationService.NavigateAsync(SettingsNavigationUri)).Success)
                        {
                            PreviousItem = selectedItem;
                            base.SelectedItem = selectedItem;
                            SettingsInvoked?.Invoke(this, EventArgs.Empty);
                        }
                        else
                        {
                            base.SelectedItem = null;
                        }
                    }
                    else
                    {
                        PreviousItem = selectedItem;
                        base.SelectedItem = selectedItem;
                    }


                }
            }
            else if (selectedItem is NavigationViewItem item)
            {
                if (item.GetValue(NavViewProps.NavigationUriProperty) is string path)
                {
			        if (!withNavigation)
			        {
				        PreviousItem = item;
				        base.SelectedItem = item;
			        }
			        else if ((await NavigationService.NavigateAsync(path)).Success)
			        {
				        PreviousItem = selectedItem;
				        base.SelectedItem = selectedItem;
			        }
			        else
			        {
				        base.SelectedItem = PreviousItem;
				        Debug.WriteLine($"{selectedItem}.{nameof(NavViewProps.NavigationUriProperty)} navigation failed.");
			        }
                }
                else
                {
                    Debug.WriteLine($"{selectedItem}.{nameof(NavViewProps.NavigationUriProperty)} is not valid Uri");
                }
            }
        }

        private bool TryFindItem(Type type,object parameter, out object item)
        {
            // registered?

            if (!PageRegistry.TryGetRegistration(type, out var info))
            {
                item = null;
                return false;
            }

            // search settings

            if (NavigationQueue.TryParse(SettingsNavigationUri, null, out var settings))
            {
                if (type == settings.Last().View && (string)parameter == settings.Last().QueryString)
                {
                    item = SettingsItem;
                    return true;
                }
                else
                {
                    // not settings
                }
            }

            // filter menu items

            var menuItems = MenuItems
                .OfType<NavigationViewItem>()
                .Select(x => new
                {
                    Item = x,
                    Path = x.GetValue(NavViewProps.NavigationUriProperty) as string
                })
                .Where(x => !string.IsNullOrEmpty(x.Path));

            // search filtered items

            foreach (var menuItem in menuItems)
            {
                if (NavigationQueue.TryParse(menuItem.Path, null, out var menuQueue)
                    && Equals(menuQueue.Last().View, type) && menuQueue.Last().QueryString == (string)parameter)
                {
                    item = menuItem.Item;
                    return true;
                }
            }

            // not found

            item = null;
            return false;
        }

        private NavigationViewItem Find(NavigationViewItem item)
        {
            return this.MenuItems.OfType<NavigationViewItem>().SingleOrDefault(x => x.Equals(item));
        }
    }
}
