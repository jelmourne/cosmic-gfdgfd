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

namespace cosmic_management_system.View.UserPage {
    public partial class ArtistsPage : Page {
        public ArtistsPage() {
            InitializeComponent();
        }
        // Connect Adapter/Connection: creating connection adapter
        public static NpgsqlConnection con;

        // Command Adapter Execute Query: creating object of the command adapter
        public static NpgsqlCommand cmd;

        // ADD ARTIST TO DB
        private void AddArtistBtn_Click(object sender, RoutedEventArgs e)
        {
            string Query;
            string firstName = artistFirstNameBox.Text;
            string lastName = artistLastNameBox.Text;
            string stageName = artistStageNameBox.Text;
            string country = artistCountryBox.Text;
            string city = artistCityBox.Text;
            string postal = artistPostalBox.Text;
            string address_line1 = artistAddress1Box.Text;
            string address_line2 = artistAddress2Box.Text;
            string biography = artistBiographyBox.Text;

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(artistDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }


            // Establish DB Connection
            EstablishConnection();
            con.Open();

            int country_id = GetCountryId(country);
            int city_id = GetCityId(city);

           if (city_id == -1)
            {
                if (country_id == -1)
                {
                    MessageBox.Show("Invalid country");
                }
                else
                {
                    Query = "INSERT INTO festival.city(city, country_id) VALUES (@city, @country_id)";
                    try
                    {
                        using (cmd = new NpgsqlCommand(Query, con))
                        {
                            cmd.Parameters.AddWithValue("@city", city);
                            cmd.Parameters.AddWithValue("@country_id", country_id);
                            cmd.ExecuteNonQuery();
                        }
                    } catch (NpgsqlException ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            } else
            {
                Query = "INSERT INTO festival.address(address_line1, address_line2, district, postal, phone, city_id) VALUES (@address_line1, @address_line2, @district, @postal, @phone, @city_id)";
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@address_line1", address_line1);
                    cmd.Parameters.AddWithValue("@address_line2", address_line1);
                    cmd.Parameters.AddWithValue("@district", "Hiii");
                    cmd.Parameters.AddWithValue("@postal", postal);
                    cmd.Parameters.AddWithValue("@phone", "6137918927");
                    cmd.Parameters.AddWithValue("@city_id", city_id);
                    cmd.ExecuteNonQuery();
                }
            }

            int address_id = GetAddressId(address_line1, city);

            try
            {
                Query = "INSERT INTO festival.artist VALUES(@first_name, @last_name, @stage_name, @dob, @biography, @address_id)";
                using (var cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@first_name", firstName);
                    cmd.Parameters.AddWithValue("@last_name", lastName);
                    cmd.Parameters.AddWithValue("@stage_name", stageName);
                    cmd.Parameters.AddWithValue("@dob", dob);
                    cmd.Parameters.AddWithValue("@address_id", address_id);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Artist added");
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private void EstablishConnection()
        {
            try
            {
                con = new NpgsqlConnection(GetConnectionString());
                MessageBox.Show("Database Connection Established");
            }
            catch (NpgsqlException ex) { 
                MessageBox.Show(ex.Message);
            }

        }

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

        private int GetCountryId(string country)
        {
            string Query = "SELECT country_id FROM festival.country WHERE country=@country";
            int country_id = -1;

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@country", country);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            country_id = reader.GetInt32(0);
                        }
                    }
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            return country_id;
        }

        private int GetCityId(string city)
        {
            string Query = "SELECT city_id FROM festival.city WHERE city = @city";
            int city_id = -1;

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@city", city);
                    using (NpgsqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            city_id = reader.GetInt32(0);
                        }
                    }
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return city_id;
        }

        private int GetAddressId(string address_line1, string city)
        {
            string Query = "SELECT address_id FROM festival.address WHERE address_line1 = @address_line AND city = @city";
            int address_id = -1;

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@address_line", address_line1);
                    cmd.Parameters.AddWithValue("@city", city);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        address_id = reader.GetInt32(0);
                    }
                }
            } catch (NpgsqlException ex) 
            { 
                MessageBox.Show(ex.Message);
            }
            return address_id;
        }
    }
}
