using cosmic_management_system.View.UserPage;
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


namespace cosmic_management_system.View.UserControls {
    /// <summary>
    /// Interaction logic for NavButton.xaml
    /// </summary>
    public partial class NavButton : UserControl {
        public NavButton() {
            InitializeComponent();
        }

        public string Title {
            get { return (string)GetValue(title); }
            set { SetValue(title, value); }
        }

        public static readonly DependencyProperty title =
            DependencyProperty.Register("Title", typeof(string), typeof(NavButton), new PropertyMetadata(string.Empty));


       
        public ImageSource Image {
            get { return (ImageSource)GetValue(image); }
            set { SetValue(image, value); }
        }

        public static readonly DependencyProperty image =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(NavButton), new PropertyMetadata());
   


        private void Button_Click(object sender, RoutedEventArgs e) {
           
        }
    }
}
