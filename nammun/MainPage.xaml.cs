using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Input;
using nammun.ViewModels;
using System.Windows.Media;

namespace nammun
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Set the data context of the listbox control to the sample data
            DataContext = App.ViewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
        }

        private void OilLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemViewModel item = OilLongListSelector.SelectedItem as ItemViewModel;
            if (item != null)
            {
                OilLongListSelector.SelectedItem = null;
                string name = item.LineOne;
                NavigationService.Navigate(new Uri("/PricesPage.xaml?type=oil&name=" + name, UriKind.Relative));
            }
        }

        private void LpgLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemViewModel item = LpgLongListSelector.SelectedItem as ItemViewModel;
            if (item != null)
            {
                LpgLongListSelector.SelectedItem = null;
                string name = item.LineOne;
                NavigationService.Navigate(new Uri("/PricesPage.xaml?type=lpg&name=" + name, UriKind.Relative));
            }
        }

        private void NgvLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ItemViewModel item = NgvLongListSelector.SelectedItem as ItemViewModel;
            if (item != null)
            {
                NgvLongListSelector.SelectedItem = null;
                string name = item.LineOne;
                NavigationService.Navigate(new Uri("/PricesPage.xaml?type=ngv&name=" + name, UriKind.Relative));
            }
        }
    }
}