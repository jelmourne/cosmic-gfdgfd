using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
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
    /// Interaction logic for SchedulePage.xaml
    /// </summary>
    public partial class SchedulePage : Page {
        public SchedulePage() {
            InitializeComponent();
        }

        public static NpgsqlConnection con;

        // Command Adapter Execute Query: creating object of the command adapter
        public static NpgsqlCommand cmd;

        private void Schedule_SelectedDatesChanged(object sender, SelectionChangedEventArgs e) {
            EstablishConnection();
            con.Open();
            DateTime date = new DateTime(long.Parse(Schedule.SelectedDate.ToString()));
            MessageBox.Show(date.ToString());

            string Query = "SELECT * FROM festival.artist WHERE dob=@dob";
            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@dob", date);
                    NpgsqlDataAdapter da = new NpgsqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    Events.ItemsSource = dt.AsDataView();
                    DataContext = da;
                    con.Close();
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        

        private void EstablishConnection() {
            try {
                con = new NpgsqlConnection(GetConnectionString());
                MessageBox.Show("Database Connection Established");
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
        }

        // Generate connection string for DB
        private string GetConnectionString() {
            string host = "Host=localhost;";
            string port = "Port=5433;";
            string dbName = "Database=cosmic_management;";
            string userName = "Username=postgres;";
            string password = "Password=Bruce-12;";
            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }
    }
}
