﻿using System;
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
            MainPage.Content = new HomePage();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e) {
            
        }

        private void Button_Click(object sender, RoutedEventArgs e) {

        }
        
        


    }
}
