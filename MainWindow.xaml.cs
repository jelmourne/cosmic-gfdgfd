using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using cosmic_management_system.View.UserPage;


namespace cosmic_management_system {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            
            InitializeComponent();
            User user = new User();
            user.name = "Justin";
            user.admin = false;

            if(user.name != null) {
                if (user.name.Length > 10) {
                    UserId.Text = "Hi, "+user.name.Substring(0, 10);
                }
                else {
                    UserId.Text = "Hi, " + user.name;
                }

                var IsAdmin = (user.admin == true) ? AccountType.Text = "Admin" : AccountType.Text = "User";
         
            }
            else {
                UserId.Text = "Login";
            }
            MainPage.Content = new HomePage();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new HomePage();
        }

        private void AttendiesButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new AttendiesPage();
        }

        private void ArtistButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new ArtistsPage();
        }

        private void MapButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new MapPage();
        }

        private void ScheduleButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new SchedulePage();
        }

        private void ProdButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new ProdPage();
        }

        private void VendorsButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new VendorPage();
        }
    }
    public class User {
        public string name { get; set; }
        public bool admin {  get; set; }
    }
}
