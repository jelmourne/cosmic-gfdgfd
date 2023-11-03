using cosmic_management_system.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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

    public partial class ProdPage : Page {
        HttpClient client = new HttpClient();
        public ProdPage() {
            client.BaseAddress = new Uri("https://localhost:7211/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json") );
            InitializeComponent();
        }

        private async void addStageBtn_Click(object sender, RoutedEventArgs e)
        {
            Stage stage = new Stage();

            stage.name = stageNameBox.Text;
            stage.genre = stageGenreBox.Text;
            stage.size = stageSizeBox.Text;

            var server_response = await client.PostAsJsonAsync("AddStage", stage);

            MessageBox.Show(server_response.ToString());
        }
    }
}
