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

        private MainWindow mainWindow;

        HttpClient client = new HttpClient();
        public LoginPage(MainWindow mainWindow) {
            client.BaseAddress = new Uri("https://localhost:7211/Production/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            InitializeComponent();
            this.mainWindow = mainWindow;
        }


        private async void Login_Button(object sender, RoutedEventArgs e) {
            var server_response = await client.GetStringAsync("loginUser/"+Username.Text);   
            Response<User> response_json = JsonConvert.DeserializeObject<Response<User>>(server_response);
            if(response_json.status == 200) {
                User user = new User();
                if (response_json.body.Password == Password.Text) {
                    user.Id = response_json.body.Id;
                    user.Username = response_json.body.Username;
                    user.Admin = response_json.body.Admin;
                    user.Name = response_json.body.Name;
                    user.Password = response_json.body.Password;
                    mainWindow.updateUser(user);
                }
                else {
                    MessageBox.Show("Invalid Username or Password");
                }
            }      
        }
    }
}
