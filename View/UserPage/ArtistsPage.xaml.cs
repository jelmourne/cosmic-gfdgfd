using cosmic_management_system.Models;
using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
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
            GetArtistList();
        }
        public static NpgsqlConnection con;
        public static NpgsqlCommand cmd;
        List<Artist> artists;

        // ---------- Add Artist to Database ---------- //
        private void AddArtistBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = artistFirstNameBox.Text;
            string lastName = artistLastNameBox.Text;
            string stageName = artistStageNameBox.Text;
            string country = artistCountryBox.Text;
            string city = artistCityBox.Text;
            string postal = artistPostalBox.Text;
            string address_line1 = artistAddress1Box.Text;
            string address_line2 = artistAddress2Box.Text;
            string district = artistDistrictBox.Text;
            string biography = artistBiographyBox.Text;
            string phone = artistPhoneBox.Text;


            // Need to create function to verify dob is of correct format
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
                    Query = "INSERT INTO festival.city(city, country_id) " +
                            "VALUES (@city, @country_id)";
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
            }
            city_id = GetCityId(city);
            Query = "INSERT INTO festival.address(address_line1, address_line2, district, postal, phone, city_id) " +
                    "VALUES (@addressLine1, @address_line2, @district, @postal, @phone, @city_id)";
            using (cmd = new NpgsqlCommand(Query, con))
            {
                cmd.Parameters.AddWithValue("@addressLine1", address_line1);
                cmd.Parameters.AddWithValue("@address_line2", address_line2);
                cmd.Parameters.AddWithValue("@district", district);
                cmd.Parameters.AddWithValue("@postal", postal);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@city_id", city_id);
                cmd.ExecuteNonQuery();
            }
            con.Close();

            // Get address ID to insert into artist table as foreign key
            int address_id = GetAddressId(address_line1, city_id);

            con.Open();
            try
            {
                Query = "INSERT INTO festival.artist(first_name, last_name, stage_name, dob, biography, address_id) " +
                        "VALUES(@firstName, @lastName, @stage_name, @dob, @biography, @address_id)";
                using (var cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@stage_name", stageName);
                    cmd.Parameters.AddWithValue("@dob", dob);
                    cmd.Parameters.AddWithValue("@biography", biography);
                    cmd.Parameters.AddWithValue("@address_id", address_id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    Artist newArtist = new Artist();
                    InitializeNewArtist(newArtist, dob);
                    artists.Add(newArtist);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Artist added");
                    }
                    else
                    {
                        MessageBox.Show("Unable to add artist");
                    }
                }
            } catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            GetArtistList();
            DisplayArtistInfo(artists[artists.Count - 1]);
        }

        // ---------- Remove Artist From Database ---------- //
        private void DeleteArtistBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = artistFirstNameBox.Text;
            string lastName = artistLastNameBox.Text;

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(artistDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            EstablishConnection();
            con.Open();

            string delArtistQuery = "DELETE from festival.artist " +
                                    "WHERE first_name = @firstName " +
                                    "AND last_name = @lastName AND dob = @dob";

            int addressId = GetAddressId(firstName, lastName, dob);
            string delAddrQuery = "DELETE from festival.address " +
                                  "WHERE address_id = @addressId";
            try
            {
                using (NpgsqlCommand delArtistCmd = new NpgsqlCommand(delArtistQuery, con))
                {
                    delArtistCmd.Parameters.AddWithValue("@firstName", firstName);
                    delArtistCmd.Parameters.AddWithValue("@lastName", lastName);
                    delArtistCmd.Parameters.AddWithValue("@dob", dob);
                    delArtistCmd.ExecuteNonQuery();
                }

                using (NpgsqlCommand delAddrCmd = new NpgsqlCommand(delAddrQuery, con))
                {
                    delAddrCmd.Parameters.AddWithValue("@addressId", addressId);

                    int rowsAffected = delAddrCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Artist deleted");
                    }
                    else
                    {
                        MessageBox.Show("Unable to remove artist");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            ClearFields();
            GetArtistList();
        }

        // ---------- Select Artist From Database ---------- //
        private void SelectArtistBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = artistFirstNameBox.Text;
            string lastName = artistLastNameBox.Text;

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(artistDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            EstablishConnection();
            con.Open();

            string Query = "SELECT * FROM festival.artist INNER JOIN festival.address a on a.address_id = artist.address_id " +
                           "INNER JOIN festival.city c on c.city_id = a.city_id " +
                           "INNER JOIN festival.country c2 on c.country_id = c2.country_id " +
                           "WHERE first_name = @firstName AND last_name = @lastName AND dob = @dob ";

            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@dob", dob);

                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    bool dataFound = false;
                    while (reader.Read())
                    {
                        dataFound = true;
                        artistFirstNameBox.Text = reader["first_name"].ToString();
                        artistLastNameBox.Text = reader["last_name"].ToString();
                        artistStageNameBox.Text = reader["stage_name"].ToString();
                        artistDobBox.Text = reader["dob"].ToString();
                        artistAddress1Box.Text = reader["address_line1"].ToString();
                        artistAddress2Box.Text = reader["address_line2"].ToString();
                        artistDistrictBox.Text = reader["district"].ToString();
                        artistPostalBox.Text = reader["postal"].ToString();
                        artistCountryBox.Text = reader["country"].ToString();
                        artistCityBox.Text = reader["city"].ToString();
                        artistBiographyBox.Text = reader["biography"].ToString();
                        artistPhoneBox.Text = reader["phone"].ToString();
                    }

                    if (!dataFound)
                    {
                        MessageBox.Show("No artist found");
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        // ---------- Update Artist In Database ---------- //
        private void UpdateArtistBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = artistFirstNameBox.Text;
            string lastName = artistLastNameBox.Text;
            Artist selectedArtist = artists[0];

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(artistDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            foreach (Artist artist in artists)
            {
                if (artist.FirstName == firstName)
                {
                    selectedArtist = artist;
                    break;
                }
            }

            EstablishConnection();
            con.Open();

            try
            {

                string artistQuery = "UPDATE festival.artist " +
                                     "SET stage_name = @stageName, biography = @biography " +
                                     "WHERE first_name = @firstName " +
                                     "AND last_name = @lastName " +
                                     "AND dob = @dob";

                using (NpgsqlCommand artistCmd = new NpgsqlCommand(artistQuery, con))
                {
                    artistCmd.Parameters.AddWithValue("@stageName", artistStageNameBox.Text);
                    artistCmd.Parameters.AddWithValue("@biography", artistBiographyBox.Text);
                    artistCmd.Parameters.AddWithValue("@firstName", firstName);
                    artistCmd.Parameters.AddWithValue("@lastName", lastName);
                    artistCmd.Parameters.AddWithValue("@dob", dob);
                    artistCmd.ExecuteNonQuery();
                }

                // Get address ID 
                int addressId = GetAddressId(firstName, lastName, dob);

                // Get city ID from database based off address
                string cityIdQuery = "SELECT city_id FROM festival.address " +
                                     "WHERE address_id = @address_id";

                int cityId;
                using (NpgsqlCommand cityIdCmd = new NpgsqlCommand(cityIdQuery, con))
                {
                    cityIdCmd.Parameters.AddWithValue("@address_id", addressId);

                    using (NpgsqlDataReader reader = cityIdCmd.ExecuteReader())
                    {
                        cityId = reader.Read() ? reader.GetInt32(0) : -1;
                    }

                }

                // Check if the artist has changed cities/countries
                bool changedCity = cityId == GetCityId(artistCityBox.Text) ? false : true;
                int countryId = GetCountryId(artistCountryBox.Text);

                if (changedCity)
                {
                    cityId = GetCityId(artistCityBox.Text);
                    if (cityId == -1) // If city is not already already in DB
                    {
                        if (countryId == -1) // If country is not found in country list in DB
                        {
                            MessageBox.Show("Invalid country");
                        }
                        else // If country is successfully found, insert city with country ID to DB
                        {
                            string insertCityQuery = "INSERT INTO festival.city(city, country_id) " +
                                    "VALUES (@city, @country_id)";
                            try
                            {
                                using (NpgsqlCommand insertCityCmd = new NpgsqlCommand(insertCityQuery, con))
                                {
                                    insertCityCmd.Parameters.AddWithValue("@city", artistCityBox.Text);
                                    insertCityCmd.Parameters.AddWithValue("@country_id", countryId);
                                    insertCityCmd.ExecuteNonQuery();
                                }
                            }
                            catch (NpgsqlException ex)
                            {
                                MessageBox.Show(ex.Message);
                            }
                        }

                    }
                    else // If city already exists in DB
                    {
                        string updateAddrQuery = "UPDATE festival.address " +
                                                 "SET address_line1 = @addressLine1, " +
                                                 "    address_line2 = @addressLine2, " +
                                                 "    postal = @postal, " +
                                                 "    phone = @phone, " +
                                                 "    district = @district, " +
                                                 "    city_id = @cityId " +
                                                 "WHERE address_id = @address_id";

                        using (NpgsqlCommand updateAddrCmd = new NpgsqlCommand(updateAddrQuery, con))
                        {
                            updateAddrCmd.Parameters.AddWithValue("@addressLine1", artistAddress1Box.Text);
                            updateAddrCmd.Parameters.AddWithValue("@addressLine2", artistAddress2Box.Text);
                            updateAddrCmd.Parameters.AddWithValue("@postal", artistPostalBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@phone", artistPhoneBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@district", artistDistrictBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@cityId", cityId);
                            updateAddrCmd.Parameters.AddWithValue("@address_id", addressId);
                            updateAddrCmd.ExecuteNonQuery();
                        }
                    }
                }
                InitializeNewArtist(selectedArtist, dob);

                MessageBox.Show("Artist updated");

            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }


        // ------------------------------------------------------------- //
        //                       HELPER FUNCTIONS                        //
        // ------------------------------------------------------------- //

        // Establish connection to DB
        private void EstablishConnection()
        {
            try
            {
                con = new NpgsqlConnection(GetConnectionString());
               // MessageBox.Show("Database Connection Established");
            }
            catch (NpgsqlException ex) { 
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
        // Get country ID based off country name inputted on front end
        private int GetCountryId(string country)
        {
            string Query = "SELECT country_id FROM festival.country WHERE country = @country";
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

        // Get city ID based off city name inputted on front end
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

        private int GetAddressId(string address_line1, int city_id)
        {
            // Get address ID based off address line 1 and city id 
            string Query = "SELECT address_id FROM festival.address WHERE address_line1 = @addressLine1 AND city_id = @city_id";
            int address_id = -1;

            con.Open();
            try
            {
                using (cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@addressLine1", address_line1);
                    cmd.Parameters.AddWithValue("@city_id", city_id);
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
            con.Close();
            return address_id;
        }

        private int GetAddressId(string firstName, string lastName, DateTime dob)
        {
            string addressIdQuery = "SELECT address_id FROM festival.artist " +
                                    "WHERE first_name = @firstName " +
                                    "AND last_name = @lastName " +
                                    "AND dob = @dob";

            int addressId = -1;
            using (NpgsqlCommand addressIdCmd = new NpgsqlCommand(addressIdQuery, con))
            {
                addressIdCmd.Parameters.AddWithValue("@firstName", firstName);
                addressIdCmd.Parameters.AddWithValue("@lastName", lastName);
                addressIdCmd.Parameters.AddWithValue("@dob", dob);

                using (NpgsqlDataReader reader = addressIdCmd.ExecuteReader())
                {
                    addressId = reader.Read() ? reader.GetInt32(0) : -1;
                }
            }
            return addressId;
        }

        private void ClearFields()
        {
            artistFirstNameBox.Text = "";
            artistLastNameBox.Text = "";
            artistStageNameBox.Text = "";
            artistCountryBox.Text = "";
            artistCityBox.Text = "";
            artistPostalBox.Text = "";
            artistAddress1Box.Text = "";
            artistAddress2Box.Text = "";
            artistDistrictBox.Text = "";
            artistBiographyBox.Text = "";
            artistPhoneBox.Text = "";
            artistDobBox.Text = "";
        }

        private void DisplayArtistInfo(Artist artist)
        {
            if (ArtistListView.SelectedItem != null)
            {
                artistFirstNameBox.Text = artist.FirstName;
                artistLastNameBox.Text = artist.LastName;
                artistDobBox.SelectedDate = artist.Dob;
                artistStageNameBox.Text = artist.StageName;
                artistAddress1Box.Text = artist.AddressLine1;
                artistAddress2Box.Text = artist.AddressLine2;
                artistCityBox.Text = artist.City;
                artistDistrictBox.Text = artist.District;
                artistPostalBox.Text = artist.Postal;
                artistCountryBox.Text = artist.Country;
                artistPhoneBox.Text = artist.Phone;
                artistBiographyBox.Text = artist.Biography;
            }
        }

        private List<Artist> GetArtistList()
        {
            EstablishConnection();
            con.Open();

            string Query = "SELECT first_name, " +
                           "       last_name, " +
                           "       stage_name, " +
                           "       dob, " +
                           "       address_line1, " +
                           "       address_line2, " +
                           "       city, " +
                           "       district, " +
                           "       postal, " +
                           "       country, " +
                           "       phone, " +
                           "       biography " +
                           "FROM festival.artist " +
                           "INNER JOIN festival.address a on artist.address_id = a.address_id " +
                           "INNER JOIN festival.city c on c.city_id = a.city_id " +
                           "INNER JOIN festival.country c2 on c.country_id = c2.country_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(Query, con)) 
            { 
                using (NpgsqlDataReader reader =  cmd.ExecuteReader())
                {
                    artists = new List<Artist>();

                    while (reader.Read())
                    {
                        artists.Add(new Artist()
                        {
                            FirstName = reader.GetString(0),
                            LastName = reader.GetString(1),
                            StageName = reader.GetString(2),
                            Dob = reader.GetDateTime(3),
                            AddressLine1 = reader.GetString(4),
                            AddressLine2 = reader.GetString(5),
                            City = reader.GetString(6),
                            District = reader.GetString(7),
                            Postal = reader.GetString(8),
                            Country = reader.GetString(9),
                            Phone = reader.GetString(10),
                            Biography = reader.GetString(11),
                        });
                    }
                    ArtistListView.ItemsSource = artists;
                }
                return artists;
            }

        }

        private void ArtistListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayArtistInfo(ArtistListView.SelectedItem as Artist);
        }

        private void InitializeNewArtist(Artist artist, DateTime dob)
        {
            artist.FirstName = artistFirstNameBox.Text;
            artist.LastName = artistLastNameBox.Text;
            artist.StageName = artistStageNameBox.Text;
            artist.Dob = dob;
            artist.AddressLine1 = artistAddress1Box.Text;
            artist.AddressLine2 = artistAddress2Box.Text;
            artist.City = artistCityBox.Text;
            artist.District = artistDistrictBox.Text;
            artist.Postal = artistPostalBox.Text;
            artist.Country = artistCountryBox.Text;
            artist.Phone = artistPhoneBox.Text;
            artist.Biography = artistBiographyBox.Text;
        }
    }
}
