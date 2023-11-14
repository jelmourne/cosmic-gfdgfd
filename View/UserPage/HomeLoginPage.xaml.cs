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

namespace cosmic_management_system.View.UserPage {
    /// <summary>
    /// Interaction logic for HomeLoginPage.xaml
    /// </summary>
    public partial class HomeLoginPage : Page {

        private MainWindow mainWindow;
        public HomeLoginPage(MainWindow mainWindow) {
            InitializeComponent();
            this.mainWindow = mainWindow;
            username.Text = mainWindow.user.Name;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            mainWindow.signOut();
        }
    }
}
