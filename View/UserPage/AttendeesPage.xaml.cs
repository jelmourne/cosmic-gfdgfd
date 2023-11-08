using cosmic_management_system.Models;
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
    public partial class AttendeesPage : Page {
        public AttendeesPage() {  
            InitializeComponent();
            GetAttendeesList();
        }
        public static NpgsqlConnection con;
        public static NpgsqlCommand cmd;
        List<Attendee> attendees;


        // ---------- Add Attendee to Database ---------- //
        private void AddAttendeeBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;
            string email = attenEmailBox.Text;
            string country = attenCountryBox.Text;
            string city = attenCityBox.Text;
            string postal = attenPostalBox.Text;
            string address_line1 = attenAddress1Box.Text;
            string address_line2 = attenAddress2Box.Text;
            string district = attenDistrictBox.Text;
            string phone = attenPhoneBox.Text;


            // Need to create function to verify dob is of correct format
            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(attenDobBox.Text));
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
                    }
                    catch (NpgsqlException ex)
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

            // Get address ID to insert into attendee table as foreign key
            int addressId = GetAddressId(address_line1, city_id);

            con.Open();
            try
            {
                Query = "INSERT INTO festival.customer(first_name, last_name, dob, email, address_id) " +
                        "VALUES(@firstName, @lastName, @dob, @email, @addressId)";
                using (var cmd = new NpgsqlCommand(Query, con))
                {
                    cmd.Parameters.AddWithValue("@firstName", firstName);
                    cmd.Parameters.AddWithValue("@lastName", lastName);
                    cmd.Parameters.AddWithValue("@dob", dob);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@addressId", addressId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    Attendee newAttendee = new Attendee();
                    InitializeNewAttendee(newAttendee, dob);
                    attendees.Add(newAttendee);

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Attendee added");
                    }
                    else
                    {
                        MessageBox.Show("Unable to add attendee");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            GetAttendeesList();
            DisplayAttendeeInfo(attendees[attendees.Count - 1]);
        }

        // ---------- Remove Attendee From Database ---------- //
        private void DeleteAttendeeBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(attenDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }

            EstablishConnection();
            con.Open();

            string delAttenQuery = "DELETE from festival.customer " +
                                    "WHERE first_name = @firstName " +
                                    "AND last_name = @lastName AND dob = @dob";

            int addressId = GetAddressId(firstName, lastName, dob);
            string delAddrQuery = "DELETE from festival.address " +
                                  "WHERE address_id = @addressId";
            try
            {
                using (NpgsqlCommand delAttenCmd = new NpgsqlCommand(delAttenQuery, con))
                {
                    delAttenCmd.Parameters.AddWithValue("@firstName", firstName);
                    delAttenCmd.Parameters.AddWithValue("@lastName", lastName);
                    delAttenCmd.Parameters.AddWithValue("@dob", dob);
                    delAttenCmd.ExecuteNonQuery();
                }

                using (NpgsqlCommand delAddrCmd = new NpgsqlCommand(delAddrQuery, con))
                {
                    delAddrCmd.Parameters.AddWithValue("@addressId", addressId);

                    int rowsAffected = delAddrCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Attendee deleted");
                    }
                    else
                    {
                        MessageBox.Show("Unable to remove attendee");
                    }
                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            ClearFields();
            GetAttendeesList();
        }

        // ---------- Select Attendee From Database ---------- //
        private void SelectAttendeeBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(attenDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            EstablishConnection();
            con.Open();

            string Query = "SELECT * FROM festival.customer INNER JOIN festival.address a on a.address_id = customer.address_id " +
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
                        attenFirstNameBox.Text = reader["first_name"].ToString();
                        attenLastNameBox.Text = reader["last_name"].ToString();
                        attenDobBox.Text = reader["dob"].ToString();
                        attenEmailBox.Text = reader["email"].ToString();
                        attenAddress1Box.Text = reader["address_line1"].ToString();
                        attenAddress2Box.Text = reader["address_line2"].ToString();
                        attenDistrictBox.Text = reader["district"].ToString();
                        attenPostalBox.Text = reader["postal"].ToString();
                        attenCountryBox.Text = reader["country"].ToString();
                        attenCityBox.Text = reader["city"].ToString();
                        attenPhoneBox.Text = reader["phone"].ToString();
                    }

                    if (!dataFound)
                    {
                        MessageBox.Show("No attendee found");
                    }

                }
            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            con.Close();
        }

        // ---------- Update Attendee In Database ---------- //
        private void UpdateAttendeeBtn_Click(object sender, RoutedEventArgs e)
        {
            string firstName = attenFirstNameBox.Text;
            string lastName = attenLastNameBox.Text;
            Attendee selectedAttendee = attendees[0];

            DateTime dob = DateTime.Now;
            try
            {
                dob = (DateTime.Parse(attenDobBox.Text));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            foreach (Attendee attendee in attendees)
            {
                if (attendee.FirstName == firstName)
                {
                    selectedAttendee = attendee;
                    break;
                }
            }

            EstablishConnection();
            con.Open();

            try
            {

                string attenQuery = "UPDATE festival.customer " +
                                     "SET email = @email " +
                                     "WHERE first_name = @firstName " +
                                     "AND last_name = @lastName " +
                                     "AND dob = @dob";

                using (NpgsqlCommand attenCmd = new NpgsqlCommand(attenQuery, con))
                {
                    attenCmd.Parameters.AddWithValue("@email", attenEmailBox.Text);
                    attenCmd.Parameters.AddWithValue("@firstName", firstName);
                    attenCmd.Parameters.AddWithValue("@lastName", lastName);
                    attenCmd.Parameters.AddWithValue("@dob", dob);
                    attenCmd.ExecuteNonQuery();
                }

                // Get address ID 
                int addressId = GetAddressId(firstName, lastName, dob);

                // Get city ID from database based off address
                string cityIdQuery = "SELECT city_id FROM festival.address " +
                                     "WHERE address_id = @addressId";

                int cityId;
                using (NpgsqlCommand cityIdCmd = new NpgsqlCommand(cityIdQuery, con))
                {
                    cityIdCmd.Parameters.AddWithValue("@addressId", addressId);

                    using (NpgsqlDataReader reader = cityIdCmd.ExecuteReader())
                    {
                        cityId = reader.Read() ? reader.GetInt32(0) : -1;
                    }

                }

                // Check if the attendee has changed cities/countries
                bool changedCity = cityId == GetCityId(attenCityBox.Text) ? false : true;
                int countryId = GetCountryId(attenCountryBox.Text);

                if (changedCity)
                {
                    cityId = GetCityId(attenCityBox.Text);
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
                                    insertCityCmd.Parameters.AddWithValue("@city", attenCityBox.Text);
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
                                                 "WHERE address_id = @addressId";

                        using (NpgsqlCommand updateAddrCmd = new NpgsqlCommand(updateAddrQuery, con))
                        {
                            updateAddrCmd.Parameters.AddWithValue("@addressLine1", attenAddress1Box.Text);
                            updateAddrCmd.Parameters.AddWithValue("@addressLine2", attenAddress2Box.Text);
                            updateAddrCmd.Parameters.AddWithValue("@postal", attenPostalBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@phone", attenPhoneBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@district", attenDistrictBox.Text);
                            updateAddrCmd.Parameters.AddWithValue("@cityId", cityId);
                            updateAddrCmd.Parameters.AddWithValue("@addressId", addressId);
                            updateAddrCmd.ExecuteNonQuery();
                        }
                    }
                }
                InitializeNewAttendee(selectedAttendee, dob);

                MessageBox.Show("Attendee updated");

            }
            catch (NpgsqlException ex)
            {
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
            int addressId = -1;

            con.Open();
            try {
                using (cmd = new NpgsqlCommand(Query, con)) {
                    cmd.Parameters.AddWithValue("@address_line1", address_line1);
                    cmd.Parameters.AddWithValue("@city_id", city_id);
                    NpgsqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read()) {
                        addressId = reader.GetInt32(0);
                    }
                }
            }
            catch (NpgsqlException ex) {
                MessageBox.Show(ex.Message);
            }
            con.Close();
            return addressId;
        }

        private int GetAddressId(string firstName, string lastName, DateTime dob)
        {
            string addressIdQuery = "SELECT address_id FROM festival.customer " +
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
            attenFirstNameBox.Text = "";
            attenLastNameBox.Text = "";
            attenEmailBox.Text = "";
            attenCountryBox.Text = "";
            attenCityBox.Text = "";
            attenPostalBox.Text = "";
            attenAddress1Box.Text = "";
            attenAddress2Box.Text = "";
            attenDistrictBox.Text = "";
            attenPhoneBox.Text = "";
            attenDobBox.Text = "";
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
        }

        private void InitializeNewAttendee(Attendee attendee, DateTime dob)
        {
            attendee.FirstName = attenFirstNameBox.Text;
            attendee.LastName = attenLastNameBox.Text;
            attendee.Dob = dob;
            attendee.AddressLine1 = attenAddress1Box.Text;
            attendee.AddressLine2 = attenAddress2Box.Text;
            attendee.City = attenCityBox.Text;
            attendee.District = attenDistrictBox.Text;
            attendee.Postal = attenPostalBox.Text;
            attendee.Country = attenCountryBox.Text;
            attendee.Phone = attenPhoneBox.Text;
        }

        private List<Attendee> GetAttendeesList()
        {
            EstablishConnection();
            con.Open();

            string Query = "SELECT first_name, " +
                           "       last_name, " +
                           "       dob, " +
                           "       email, " +
                           "       address_line1, " +
                           "       address_line2, " +
                           "       city, " +
                           "       district, " +
                           "       postal, " +
                           "       country, " +
                           "       phone " +
                           "FROM festival.customer " +
                           "INNER JOIN festival.address a on customer.address_id = a.address_id " +
                           "INNER JOIN festival.city c on c.city_id = a.city_id " +
                           "INNER JOIN festival.country c2 on c.country_id = c2.country_id";

            using (NpgsqlCommand cmd = new NpgsqlCommand(Query, con))
            {
                using (NpgsqlDataReader reader = cmd.ExecuteReader())
                {
                    attendees = new List<Attendee>();

                    while (reader.Read())
                    {
                        attendees.Add(new Attendee()
                        {
                            FirstName = reader.GetString(0),
                            LastName = reader.GetString(1),
                            Dob = reader.GetDateTime(2),
                            Email = reader.GetString(3),
                            AddressLine1 = reader.GetString(4),
                            AddressLine2 = reader.GetString(5),
                            City = reader.GetString(6),
                            District = reader.GetString(7),
                            Postal = reader.GetString(8),
                            Country = reader.GetString(9),
                            Phone = reader.GetString(10),
                        });
                    }
                    AttendeesListView.ItemsSource = attendees;
                }
                return attendees;
            }
        }

        private void AttendeeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayAttendeeInfo(AttendeesListView.SelectedItem as Attendee);
        }

        private void DisplayAttendeeInfo(Attendee attendee)
        {
            if (AttendeesListView.SelectedItem != null)
            {
                attenFirstNameBox.Text = attendee.FirstName;
                attenLastNameBox.Text = attendee.LastName;
                attenDobBox.SelectedDate = attendee.Dob;
                attenEmailBox.Text = attendee.Email;
                attenAddress1Box.Text = attendee.AddressLine1;
                attenAddress2Box.Text = attendee.AddressLine2;
                attenCityBox.Text = attendee.City;
                attenDistrictBox.Text = attendee.District;
                attenPostalBox.Text = attendee.Postal;
                attenCountryBox.Text = attendee.Country;
                attenPhoneBox.Text = attendee.Phone;
            }
        }
    
    }
}
