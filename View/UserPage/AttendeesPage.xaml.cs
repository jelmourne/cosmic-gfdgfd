using Npgsql;
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
using Npgsql;

namespace cosmic_management_system.View.UserPage {
    /// <summary>
    /// Interaction logic for AttendiesPage.xaml
    /// </summary>
    public partial class AttendiesPage : Page {
        public AttendiesPage() {
            InitializeComponent();
        }
        // Connect Adapter/Connection: creating connection adapter
        public static NpgsqlConnection con;

        // Command Adapter Execute Query: creating object of the command adapter
        public static NpgsqlCommand cmd;


        // ---------- Add Artist to Database ---------- //
        private void AddAttenBtn_Click(object sender, RoutedEventArgs e) {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;
            string phone = attenPhoneBox.Text;
            string email = attenEmailBox.Text;
            string country = attenCountryBox.Text;
            string city = attenCityBox.Text;
            string postal = attenPostalBox.Text;
            string address_line2 = attenAddress2Box.Text;
            string address_line1 = attenAddress1Box.Text == "" ? null : attenAddress1Box.Text;

            // Need to create function to verify dob is of correct format
            DateTime dob = DateTime.Now;
            try {
                dob = (DateTime.Parse(attenDobBox.Text));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);

            }

            // Establish DB Connection
            EstablishConnection();
            con.Open();

            string Query;
            int country_id = GetCountryId(country);
            int city_id = GetCityId(city);

            if (city_id == -1) // If city is not already already in DB
             {
                if (country_id == -1) // If country is not found in country list in DB
                {
                    MessageBox.Show("Invalid country");
                }
                else // If country is successfully found, insert city with country ID to DB
                {
                    Query = "INSERT INTO festival.city(city, country_id) VALUES (@city, @country_id)";
                    try {
                        using (cmd = new NpgsqlCommand(Query, con)) {
                            cmd.Parameters.AddWithValue("@city", city);
                            cmd.Parameters.AddWithValue("@country_id", country_id);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    catch (NpgsqlException ex) {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
            else // If city is already in DB, insert address with city ID to DB
            {
                Query = "INSERT INTO festival.address(address_line1, address_line2, postal, city_id, phone) VALUES (@address_line1, @address_line2, @postal, @city_id, @phone)";
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@address_line1", address_line1);
                    cmd.Parameters.AddWithValue("@address_line2", address_line2);
                    //cmd.Parameters.AddWithValue("@district", "District");
                    cmd.Parameters.AddWithValue("@postal", postal);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@city_id", city_id);
                    cmd.ExecuteNonQuery();
                }
            }
            con.Close();

            // Get address ID to insert into artist table as foreign key
            int address_id = GetAddressId(address_line1, city_id);
            MessageBox.Show(address_id.ToString());

            con.Open();
            try {
                Query = "INSERT INTO festival.customer(first_name, last_name, dob, email, phone, address_id) VALUES(@first_name, @last_name, @dob, @email, @phone, @address_id)";
                using (var cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@dob", dob);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@address_id", address_id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0) {
                        MessageBox.Show("Attendee added");
                    }
                    else {
                        MessageBox.Show("Unable to add attendee");
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        // ---------- Remove Artist From Database ---------- //
        private void DeleteAttenBtn_Click(object sender, RoutedEventArgs e) {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;

            DateTime dob = DateTime.Now;
            try {
                dob = (DateTime.Parse(attenDobBox.Text));
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);

            }

            EstablishConnection();
            con.Open();

            string Query = "DELETE from festival.customer WHERE first_name=@first_name AND last_name=@last_name AND dob=@dob";
            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@dob", dob);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0) {
                        MessageBox.Show("Attendee deleted");
                    }
                    else {
                        MessageBox.Show("Unable to remove attendee");
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            con.Close();

        }

        // ------------------------------------------------------------- //
        //                       HELPER FUNCTIONS                        //
        // ------------------------------------------------------------- //

        // Establish connection to DB
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
            string password = "Password=1234;";
            string connectionString = string.Format("{0}{1}{2}{3}{4}", host, port, dbName, userName, password);
            return connectionString;
        }
        // Get country ID based off country name inputted on front end
        private int GetCountryId(string country) {
            string Query = "SELECT country_id FROM festival.country WHERE country=@country";
            int country_id = -1;

            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@country", country);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            country_id = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            return country_id;
        }

        // Get city ID based off city name inputted on front end
        private int GetCityId(string city) {
            string Query = "SELECT city_id FROM festival.city WHERE city = @city";
            int city_id = -1;

            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@city", city);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader()) {
                        if (reader.Read()) {
                            city_id = reader.GetInt32(0);
                        }
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            return city_id;
        }

        private int GetAddressId(string address_line1, int city_id) {
            // Get address ID based off address line 1 and city id 
            string Query = "SELECT address_id FROM festival.address WHERE address_line1 = @address_line1 AND city_id = @city_id";
            int address_id = -1;

            con.Open();
            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@address_line1", address_line1);
                    cmd.Parameters.AddWithValue("@city_id", city_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {
                        address_id = reader.GetInt32(0);
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            return address_id;
        }
    }
}
