using CafeBakery.Model;
using CafeBakery.Views.Windows;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Pages
{
    public partial class PositionsListPage : Page
    {
        private string connectionString = DatabaseConfig.ConnectionString;
        private ObservableCollection<Position> positions = new ObservableCollection<Position>();

        public PositionsListPage()
        {
            InitializeComponent();
            LoadPositions();
        }

        private void LoadPositions()
        {
            positions.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT position_id, position_name, salary, responsibilities, requirements FROM positions", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var position = new Position
                    {
                        PositionId = (int)reader["position_id"],
                        PositionName = reader["position_name"].ToString(),
                        Salary = (decimal)reader["salary"],
                        Responsibilities = reader["responsibilities"].ToString(),
                        Requirements = reader["requirements"].ToString()
                    };
                    positions.Add(position);
                }
            }
            listViewPositions.ItemsSource = positions;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredPositions = positions.Where(p => p.PositionName.ToLower().Contains(searchText)).ToList();
            listViewPositions.ItemsSource = filteredPositions;
        }

        private void listViewPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PositionManagementWindow();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO positions (position_name, salary, responsibilities, requirements) VALUES (@position_name, @salary, @responsibilities, @requirements)",
                        connection);
                    command.Parameters.AddWithValue("@position_name", dialog.Position.PositionName);
                    command.Parameters.AddWithValue("@salary", dialog.Position.Salary);
                    command.Parameters.AddWithValue("@responsibilities", dialog.Position.Responsibilities);
                    command.Parameters.AddWithValue("@requirements", dialog.Position.Requirements);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadPositions();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listViewPositions.SelectedItem is Position selectedPosition)
            {
                var dialog = new PositionManagementWindow(selectedPosition);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE positions SET position_name = @position_name, salary = @salary, responsibilities = @responsibilities, requirements = @requirements WHERE position_id = @position_id",
                            connection);
                        command.Parameters.AddWithValue("@position_id", selectedPosition.PositionId);
                        command.Parameters.AddWithValue("@position_name", dialog.Position.PositionName);
                        command.Parameters.AddWithValue("@salary", dialog.Position.Salary);
                        command.Parameters.AddWithValue("@responsibilities", dialog.Position.Responsibilities);
                        command.Parameters.AddWithValue("@requirements", dialog.Position.Requirements);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadPositions();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewPositions.SelectedItem is Position selectedPosition)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM positions WHERE position_id = @position_id", connection);
                    command.Parameters.AddWithValue("@position_id", selectedPosition.PositionId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadPositions();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
