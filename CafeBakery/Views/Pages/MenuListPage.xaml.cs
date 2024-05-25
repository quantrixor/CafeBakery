using CafeBakery.Model;
using CafeBakery.Views.Windows;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Pages
{
    public partial class MenuListPage : Page
    {
        private string connectionString = DatabaseConfig.ConnectionString;
        private ObservableCollection<Model.Menu> menuItems = new ObservableCollection<Model.Menu>();

        public MenuListPage()
        {
            InitializeComponent();
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            menuItems.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT dish_id, dish_name, cost, preparation_time FROM menu", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var menu = new Model.Menu
                    {
                        DishId = (int)reader["dish_id"],
                        DishName = reader["dish_name"].ToString(),
                        Cost = (decimal)reader["cost"],
                        PreparationTime = (TimeSpan)reader["preparation_time"]
                    };
                    menuItems.Add(menu);
                }
            }
            listViewMenu.ItemsSource = menuItems;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredMenu = menuItems.Where(m => m.DishName.ToLower().Contains(searchText)).ToList();
            listViewMenu.ItemsSource = filteredMenu;
        }

        private void listViewMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MenuManagementWindow();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO menu (dish_name, cost, preparation_time) VALUES (@dish_name, @cost, @preparation_time)",
                        connection);
                    command.Parameters.AddWithValue("@dish_name", dialog.Menu.DishName);
                    command.Parameters.AddWithValue("@cost", dialog.Menu.Cost);
                    command.Parameters.AddWithValue("@preparation_time", dialog.Menu.PreparationTime);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadMenuItems();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listViewMenu.SelectedItem is Model.Menu selectedMenu)
            {
                var dialog = new MenuManagementWindow(selectedMenu);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE menu SET dish_name = @dish_name, cost = @cost, preparation_time = @preparation_time WHERE dish_id = @dish_id",
                            connection);
                        command.Parameters.AddWithValue("@dish_id", selectedMenu.DishId);
                        command.Parameters.AddWithValue("@dish_name", dialog.Menu.DishName);
                        command.Parameters.AddWithValue("@cost", dialog.Menu.Cost);
                        command.Parameters.AddWithValue("@preparation_time", dialog.Menu.PreparationTime);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadMenuItems();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewMenu.SelectedItem is Model.Menu selectedMenu)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM menu WHERE dish_id = @dish_id", connection);
                    command.Parameters.AddWithValue("@dish_id", selectedMenu.DishId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadMenuItems();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
