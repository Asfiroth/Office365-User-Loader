using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Office365.User.Loader.Properties;


namespace Office365.User.Loader.Services
{
    public class NavigationService : ILoaderNavigationService
    {
        #region Members
        private readonly Dictionary<string, Uri> _pagesByKey;
        private readonly List<string> _backstack;
        public string CurrentPageKey { get; private set; }
        public object Parameter { get; private set; }

        #endregion

        #region Constructor
        public NavigationService()
        {
            _pagesByKey = new Dictionary<string, Uri>();
            _backstack = new List<string>();
        }
        #endregion

        #region Navigation Methods
        public void GoBack()
        {
            if (_backstack.Count <= 1) return;
            _backstack.RemoveAt(_backstack.Count - 1);
            NavigateTo(_backstack.Last(), null);
        }

        public void NavigateTo(string pageKey)
        {
            NavigateTo(pageKey, null);
        }

        public void NavigateTo(string pageKey, object parameter)
        {
            lock (_pagesByKey)
            {
                if (!_pagesByKey.ContainsKey(pageKey))
                {
                    throw new ArgumentException(string.Format(Resources.NoPageException, pageKey));
                }
                var frame = GetDescendantFromName(Application.Current.MainWindow, "ContentFrame") as Frame;

                if(frame == null) throw new NullReferenceException(Resources.NoFrame);
                frame.Source = _pagesByKey[pageKey];
                Parameter = parameter;
                _backstack.Add(pageKey);
                CurrentPageKey = pageKey;
            }
        }

        public void Configure(string pageKey, Uri pageType)
        {
            lock (_pagesByKey)
            {
                if (_pagesByKey.ContainsKey(pageKey))
                {
                    _pagesByKey[pageKey] = pageType;
                }
                else
                {
                    _pagesByKey.Add(pageKey, pageType);
                }
            }
        }
        #endregion

        #region Private Methods

        private static FrameworkElement GetDescendantFromName(DependencyObject parent, string name)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            if (count < 1) return null;
            for (var i = 0; i < count; i++)
            {
                var frameworkElement = VisualTreeHelper.GetChild(parent, i) as FrameworkElement;
                if(frameworkElement == null) continue;
                if (frameworkElement.Name == name) return frameworkElement;
                frameworkElement = GetDescendantFromName(frameworkElement, name);
                if (frameworkElement != null) return frameworkElement;
            }
            return null;
        }
        #endregion
    }
}