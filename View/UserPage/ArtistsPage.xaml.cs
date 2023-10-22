using System;
using System.Collections.Generic;
using System.Data;
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
using Npgsql;

namespace cosmic_management_system.View.UserPage {
    /// <summary>
    /// Interaction logic for ArtistsPage.xaml
    /// </summary>
    public partial class ArtistsPage : Page {
        public ArtistsPage() {
            InitializeComponent();
            establishConnection();

            con.Open();
            string Query = "SELECT * FROM festival.artist";
            cmd = new NpgsqlCommand(Query, con);

            NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            ArtistList.ItemsSource = dt.AsDataView();
            DataContext = da;
            con.Close();

        }
        private static NpgsqlConnection con;
        private static NpgsqlCommand cmd;

        private string get_ConnectionString() {
            string host = "Host=localhost;";
            string port = "Port=5432;";
            string dbname = "Database=cosmic-management;";
            string userName = "Username=postgres;";
            string password = "Password=Bruce-12;";

            string ConnectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbname, userName, password);
            return ConnectionString;
        }

        private void establishConnection() {
            try {
                con = new NpgsqlConnection(get_ConnectionString());
            }
            catch (NpgsqlException e) { MessageBox.Show(e.ToString()); }
        }


    }
}
