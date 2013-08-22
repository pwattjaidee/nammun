using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using nammun.ViewModels;
using System.Collections.ObjectModel;

namespace nammun
{
    public partial class PricesPage : PhoneApplicationPage
    {
        // Data from MainPage.xaml
        private string type;
        private string name;

        private static PriceViewModel viewModel = new PriceViewModel();

        // Constructor
        public PricesPage()
        {
            InitializeComponent();
            DataContext = viewModel;
        }

        // Load data for the ViewModel Items
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (this.NavigationContext.QueryString.ContainsKey("type"))
            {
                type = this.NavigationContext.QueryString["type"];
            }

            if (this.NavigationContext.QueryString.ContainsKey("name"))
            {
                name = this.NavigationContext.QueryString["name"];
            }

            pivotItemHeader.Header = name + " " + type + " prices";

            viewModel.LoadData(type, name);

            pivot.Title = viewModel.Title;
        }
    }
}