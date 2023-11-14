using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cosmic_management_system.Models;
using Newtonsoft.Json;
using System.Collections;

namespace cosmic_management_system.View.UserPage {
    /// <summary>
    /// Interaction logic for VendorPage.xaml
    /// </summary>
    public partial class VendorPage : Page {
        HttpClient client = new HttpClient();
        public VendorPage() {
            client.BaseAddress = new Uri("https://localhost:7211/Production/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeComponent();
            getVendor();
        }

        public async void getVendor() {
            var server_response = await client.GetStringAsync("getVendor");
            Response<Vendor> responseJson = JsonConvert.DeserializeObject<Response<Vendor>>(server_response);

            VendorListView.ItemsSource = responseJson.data;
            DataContext = this;
        }

        private async void addVendorBtn_Click(object sender, RoutedEventArgs e) {
            Vendor vendor = new Vendor();
            vendor.name = vendorNameBox.Text;
            vendor.type = vendorTypeBox.Text;

            var server_response = await client.PostAsJsonAsync("insertVendor", vendor);
            getVendor();
        }

        private async void deleteAttenBtn_Click(object sender, RoutedEventArgs e) {
            Vendor vendor = new Vendor();
            vendor.id = 1;

            var server_response = await client.PostAsJsonAsync("deleteVendor", vendor);
            getVendor();
        }

        private async void updateAttenBtn_Click(object sender, RoutedEventArgs e) {
            Vendor vendor = new Vendor();
            vendor.id = 1;
            vendor.name = vendorNameBox.Text;
            vendor.type = vendorTypeBox.Text;

            var server_response = await client.PostAsJsonAsync("updateVendor", vendor);
            getVendor();
        }
    }
}
