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

namespace cosmic_management_system.View.UserPage {
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page {

        HttpClient client = new HttpClient();
        public LoginPage() {
            client.BaseAddress = new Uri("https://localhost:7211/Production/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeComponent();
        }

        private async void Login_Button(object sender, RoutedEventArgs e) {
            User user = new User();
            user.Id = 0;
            user.Name = "";
            user.Username = Username.Text;
            user.Password = Password.Text;
            user.Admin = false;

            var server_response = await client.PostAsJsonAsync("loginUser", user);
            Response response_json = JsonConvert.DeserializeObject<Response>(server_response.ToString());
            MessageBox.Show(response_json.ToString());
           
        }
    }
}
