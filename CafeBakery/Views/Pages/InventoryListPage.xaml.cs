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
    public partial class InventoryListPage : Page
    {
        private string connectionString = DatabaseConfig.ConnectionString;
        private ObservableCollection<Inventory> inventoryItems = new ObservableCollection<Inventory>();

        public InventoryListPage()
        {
            InitializeComponent();
            LoadInventoryItems();
        }

        private void LoadInventoryItems()
        {
            inventoryItems.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT ingredient_id, ingredient_name, production_date, volume, expiry_date, cost, supplier FROM inventory", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var inventory = new Inventory
                    {
                        IngredientId = (int)reader["ingredient_id"],
                        IngredientName = reader["ingredient_name"].ToString(),
                        ProductionDate = (DateTime)reader["production_date"],
                        Volume = (decimal)reader["volume"],
                        ExpiryDate = (DateTime)reader["expiry_date"],
                        Cost = (decimal)reader["cost"],
                        Supplier = reader["supplier"].ToString()
                    };
                    inventoryItems.Add(inventory);
                }
            }
            listViewInventory.ItemsSource = inventoryItems;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredInventory = inventoryItems.Where(i => i.IngredientName.ToLower().Contains(searchText)).ToList();
            listViewInventory.ItemsSource = filteredInventory;
        }

        private void listViewInventory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Here you could handle the selection changed event if needed
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new InventoryManagementWindow();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO inventory (ingredient_name, production_date, volume, expiry_date, cost, supplier) VALUES (@ingredient_name, @production_date, @volume, @expiry_date, @cost, @supplier)",
                        connection);
                    command.Parameters.AddWithValue("@ingredient_name", dialog.Inventory.IngredientName);
                    command.Parameters.AddWithValue("@production_date", dialog.Inventory.ProductionDate);
                    command.Parameters.AddWithValue("@volume", dialog.Inventory.Volume);
                    command.Parameters.AddWithValue("@expiry_date", dialog.Inventory.ExpiryDate);
                    command.Parameters.AddWithValue("@cost", dialog.Inventory.Cost);
                    command.Parameters.AddWithValue("@supplier", dialog.Inventory.Supplier);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadInventoryItems(); // Reload the inventory items to refresh the list
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listViewInventory.SelectedItem is Inventory selectedInventory)
            {
                var dialog = new InventoryManagementWindow(selectedInventory);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE inventory SET ingredient_name = @ingredient_name, production_date = @production_date, volume = @volume, expiry_date = @expiry_date, cost = @cost, supplier = @supplier WHERE ingredient_id = @ingredient_id",
                            connection);
                        command.Parameters.AddWithValue("@ingredient_id", selectedInventory.IngredientId);
                        command.Parameters.AddWithValue("@ingredient_name", dialog.Inventory.IngredientName);
                        command.Parameters.AddWithValue("@production_date", dialog.Inventory.ProductionDate);
                        command.Parameters.AddWithValue("@volume", dialog.Inventory.Volume);
                        command.Parameters.AddWithValue("@expiry_date", dialog.Inventory.ExpiryDate);
                        command.Parameters.AddWithValue("@cost", dialog.Inventory.Cost);
                        command.Parameters.AddWithValue("@supplier", dialog.Inventory.Supplier);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadInventoryItems(); // Reload the inventory items to refresh the list
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewInventory.SelectedItem is Inventory selectedInventory)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM inventory WHERE ingredient_id = @ingredient_id", connection);
                    command.Parameters.AddWithValue("@ingredient_id", selectedInventory.IngredientId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadInventoryItems();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
