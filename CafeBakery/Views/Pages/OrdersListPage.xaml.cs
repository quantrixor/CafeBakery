using CafeBakery.Model;
using CafeBakery.Views.Windows;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Pages
{
    public partial class OrdersListPage : Page
    {
        private string connectionString = DatabaseConfig.ConnectionString;
        private List<Order> orders = new List<Order>();

        public OrdersListPage()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            orders.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT order_id, order_date, order_time, customer_name, customer_phone, total_cost, completion_status, employee_id FROM orders", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var order = new Order
                    {
                        OrderId = (int)reader["order_id"],
                        OrderDate = (DateTime)reader["order_date"],
                        OrderTime = (TimeSpan)reader["order_time"],
                        CustomerName = reader["customer_name"].ToString(),
                        CustomerPhone = reader["customer_phone"].ToString(),
                        TotalCost = (decimal)reader["total_cost"],
                        CompletionStatus = reader["completion_status"].ToString(),
                        EmployeeId = (int)reader["employee_id"]
                    };
                    orders.Add(order);
                }
            }
            listViewOrders.ItemsSource = null;
            listViewOrders.ItemsSource = orders;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredOrders = orders.Where(o => o.CustomerName.ToLower().Contains(searchText)).ToList();
            listViewOrders.ItemsSource = filteredOrders;
        }

        private void listViewOrders_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: Handle selection changed logic if needed
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OrderManagementWindow();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO orders (order_date, order_time, customer_name, customer_phone, total_cost, completion_status, employee_id) " +
                        "VALUES (@order_date, @order_time, @customer_name, @customer_phone, @total_cost, @completion_status, @employee_id)",
                        connection);
                    command.Parameters.AddWithValue("@order_date", dialog.Order.OrderDate);
                    command.Parameters.AddWithValue("@order_time", dialog.Order.OrderTime);
                    command.Parameters.AddWithValue("@customer_name", dialog.Order.CustomerName);
                    command.Parameters.AddWithValue("@customer_phone", dialog.Order.CustomerPhone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@total_cost", dialog.Order.TotalCost);
                    command.Parameters.AddWithValue("@completion_status", dialog.Order.CompletionStatus ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@employee_id", dialog.Order.EmployeeId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadOrders();
            }
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listViewOrders.SelectedItem is Order selectedOrder)
            {
                var dialog = new OrderManagementWindow(selectedOrder);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE orders SET order_date = @order_date, order_time = @order_time, customer_name = @customer_name, " +
                            "customer_phone = @customer_phone, total_cost = @total_cost, completion_status = @completion_status, employee_id = @employee_id " +
                            "WHERE order_id = @order_id",
                            connection);
                        command.Parameters.AddWithValue("@order_id", selectedOrder.OrderId);
                        command.Parameters.AddWithValue("@order_date", dialog.Order.OrderDate);
                        command.Parameters.AddWithValue("@order_time", dialog.Order.OrderTime);
                        command.Parameters.AddWithValue("@customer_name", dialog.Order.CustomerName);
                        command.Parameters.AddWithValue("@customer_phone", dialog.Order.CustomerPhone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@total_cost", dialog.Order.TotalCost);
                        command.Parameters.AddWithValue("@completion_status", dialog.Order.CompletionStatus ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@employee_id", dialog.Order.EmployeeId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadOrders();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewOrders.SelectedItem is Order selectedOrder)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM orders WHERE order_id = @order_id", connection);
                    command.Parameters.AddWithValue("@order_id", selectedOrder.OrderId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadOrders();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
