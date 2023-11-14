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
using cosmic_management_system.Models;


namespace cosmic_management_system {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public User user { get; set; } = new User();

        public MainWindow() {
            InitializeComponent();
            MainPage.Navigate(new HomePage(this));
        }

       public void updateUser(User user) {
            AccountType.Text = user.Admin == true ? "Admin" : "User";
            UserId.Text = "Hi, "+user.Name;
            this.user = user;
            MainPage.Navigate(new HomeLoginPage(this));
        }

        public void signOut() {
            AccountType.Text = "";
            UserId.Text = "Please Login";
            user = new User();
            MainPage.Navigate(new HomePage(this));
        }

        public void updateFrame(Page page) {
            MainPage.Navigate(page);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Navigate(new LoginPage(this));
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e) {
            if(user.Username != null) {
                MainPage.Navigate(new HomeLoginPage(this));
            }
            else {
                MainPage.Navigate(new HomePage(this));
            }
        }

        private void AttendeesButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new AttendeesPage();
        }

        private void ArtistButton_Click(object sender, RoutedEventArgs e) {
            MainPage.Content = new ArtistsPage();
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
}
