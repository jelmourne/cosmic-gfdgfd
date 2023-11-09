using cosmic_management_system.Models;
using Newtonsoft.Json;
using Npgsql;
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
            DisplayAllStages();
            DisplayAllProduction();
        }

        public static NpgsqlConnection con;
        public static NpgsqlCommand cmd;

        private async void addStageBtn_Click(object sender, RoutedEventArgs e)
        {
            Stage stage = new Stage();

            stage.name = stageNameBox.Text;
            stage.genre = stageGenreBox.Text;
            stage.size = stageSizeBox.Text;

            var server_response = await client.PostAsJsonAsync("Stage/AddStage", stage);

            MessageBox.Show(server_response.ToString());
            DisplayAllStages();
        }

        private async void deleteStageBtn_Click(object sender, RoutedEventArgs e)
        {
            EstablishConnection();
            con.Open();
            string Query = "DELETE FROM festival.stage_prod WHERE stage_id=@stage_id";

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {

                    cmd.Parameters.AddWithValue("@stage_id", int.Parse(stageIdBox.Text));

                    int rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();

           
            var server_response = await client.DeleteAsync("Stage/DeleteStage/" + stageNameBox.Text);

            MessageBox.Show(server_response.ToString());
            stageNameBox.Text = "";
            stageIdBox.Text = "";
            DisplayAllStages();
            Clear();

        }

        private async void updateStageBtn_Click(object sender, RoutedEventArgs e)
        {
            Stage stage = new Stage();

            stage.id = int.Parse(stageIdBox.Text);
            stage.name = stageNameBox.Text;
            stage.genre = stageGenreBox.Text;
            stage.size = stageSizeBox.Text;

            var server_response = await client.PutAsJsonAsync("Stage/UpdateStage", stage);
            DisplayAllStages();
            MessageBox.Show(server_response.ToString());
            
        }

        private void removeStageProdBtn_Click(object sender, RoutedEventArgs e)
        {
            EstablishConnection();
            con.Open();

            string Query = "DELETE FROM festival.stage_prod " +
                           "WHERE stage_id=@stage_id AND prod_id=@prod_id";

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {

                    cmd.Parameters.AddWithValue("@stage_id", int.Parse(stageIdBox.Text));
                    cmd.Parameters.AddWithValue("@prod_id", (StageProdListView.SelectedItem as Production).id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Prod req removed");
                        DisplayStageReqs(StageListView.SelectedItem as Stage);

                    }
                    else
                    {
                        MessageBox.Show("Unable to remove prod req");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private void removeProdListBtn_Click(object sender, RoutedEventArgs e)
        {
            EstablishConnection();
            con.Open();

            string stageReqQuery = "DELETE FROM festival.stage_prod WHERE prod_id=@prod_id";

            try
            {
                using (NpgsqlCommand stageReqCmd = new NpgsqlCommand(stageReqQuery, con))
                {
                    stageReqCmd.Parameters.AddWithValue("@prod_id", (ProductionListView.SelectedItem as Production).id);
                    int rowsAffected = stageReqCmd.ExecuteNonQuery();
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            string Query = "DELETE FROM festival.production WHERE prod_id=@prod_id";
            
            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@prod_id", (ProductionListView.SelectedItem as Production).id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Production item removed");
                        DisplayAllProduction();
                        DisplayStageReqs(StageListView.SelectedItem as Stage);

                    }
                    else
                    {
                        MessageBox.Show("Unable to remove item");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private async void DisplayAllStages()
        {
            var server_response = await client.GetStringAsync("Stage/GetAllStages");

            StageResponse response_json = JsonConvert.DeserializeObject<StageResponse>(server_response);

            List<Stage> stages = new List<Stage>();
            stages = response_json.stages;
            StageListView.ItemsSource = stages;
            DataContext = this;
        }

        private async void DisplayAllProduction()
        {
            var server_response = await client.GetStringAsync("Production/getProduction");

            Response response_json = JsonConvert.DeserializeObject<Response>(server_response);

            List<Production> productionList = new List<Production>();
            productionList = response_json.data;
            ProductionListView.ItemsSource = productionList;
            DataContext = this;
        }

        private async void DisplayStageReqs(Stage stage)
        {
            var server_response = await client.GetStringAsync("Stage/GetStageReqs/" + stage.id);

            Response response_json = JsonConvert.DeserializeObject<Response>(server_response);

            List<Production> stageReqsList = new List<Production>();
            stageReqsList = response_json.data;
            StageProdListView.ItemsSource = stageReqsList;
            DataContext = this;
        }

        private async void addToProdListBtn_Click(object sender, RoutedEventArgs e)
        {
            Production production = new Production();

            production.type = productionItemBox.Text;
            production.description = productionDescriptionBox.Text;

            var server_response = await client.PostAsJsonAsync("Production/insertProduction", production);

            MessageBox.Show(server_response.ToString());
        }

        private async void GetStage(string name)
        {
            Stage stage = new Stage();

            var server_response = await client.GetStringAsync("/Stage/GetStageByName/" + stageNameBox.Text);

            Response response_json = JsonConvert.DeserializeObject<Response>(server_response);

            MessageBox.Show(server_response);
        }

        private void StageListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayStageInfo(StageListView.SelectedItem as Stage);
            DisplayStageReqs(StageListView.SelectedItem as Stage);
        }

        private void DisplayStageInfo(Stage stage)
        {
            if (StageListView.SelectedItem != null)
            {
                stageNameBox.Text = stage.name;
                stageIdBox.Text = stage.id.ToString();
                stageGenreBox.Text = stage.genre;
                stageSizeBox.Text = stage.size;
            }
        }


        private void DisplayProductionInfo(Production prod)
        {
            if (ProductionListView.SelectedItem != null)
            {
                productionItemBox.Text = prod.type;
                productionDescriptionBox.Text = prod.description;
            }
        }

        private void DisplayStageProdInfo(Production production)
        {
            if (StageProdListView.SelectedItem != null)
            {
                productionItemBox.Text = production.type;
                productionDescriptionBox.Text = production.description;
            }
        }

        private void ProductionListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayProductionInfo(ProductionListView.SelectedItem as Production);
        }

        private void addStageProdBtn_Click(object sender, RoutedEventArgs e)
        {
            EstablishConnection();
            con.Open();

            string Query = "INSERT INTO festival.stage_prod VALUES(@stage_id, @prod_id)";

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    Stage stage = new Stage();
                    Production prod = new Production();

                    stage = StageListView.SelectedItem as Stage;
                    prod = ProductionListView.SelectedItem as Production;

                    cmd.Parameters.AddWithValue("@stage_id", stage.id);
                    cmd.Parameters.AddWithValue("@prod_id", prod.id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Prod req added to stage");
                        DisplayStageReqs(stage);
                    }
                    else
                    {
                        MessageBox.Show("Unable to add prod req to stage");
                    }
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            
        }

        private void StageProdListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayStageProdInfo((StageProdListView.SelectedItem as Production));
        }

        // Establish connection to DB
        private void EstablishConnection()
        {
            try
            {
                con = new NpgsqlConnection(GetConnectionString());
                // MessageBox.Show("Database Connection Established");
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Generate connection string for DB
        private string GetConnectionString()
        {
            string host = "Host=localhost;";
            string port = "Port=5433;";
            string dbName = "Database=cosmic_management;";
            string userName = "Username=postgres;";
            string password = "Password=1234;";
            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            stageNameBox.Text = "";
            stageGenreBox.Text = "";
            stageIdBox.Text = "";
            stageSizeBox.Text = "";
            productionItemBox.Text = "";
            productionDescriptionBox.Text = "";
        }

        private async void updateProdItem_Click(object sender, RoutedEventArgs e)
        {
            Production prod = new Production();
            prod = ProductionListView.SelectedItem as Production;
            prod.type = productionItemBox.Text;
            prod.description = productionDescriptionBox.Text;

            var server_response = await client.PutAsJsonAsync("Production/updateProduction", prod);
            DisplayAllProduction();
            MessageBox.Show(server_response.ToString());
        }
    }
}
