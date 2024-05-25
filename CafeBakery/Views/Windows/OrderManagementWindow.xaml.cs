using CafeBakery.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Windows
{
    public partial class OrderManagementWindow : Window
    {
        public Order Order { get; private set; }
        private bool isEditMode;
        private string connectionString = DatabaseConfig.ConnectionString;
        private List<Employee> employees = new List<Employee>();

        public OrderManagementWindow()
        {
            InitializeComponent();
            Order = new Order();
            DataContext = Order;
            isEditMode = false;
            LoadEmployees();
        }

        public OrderManagementWindow(Order order) : this()
        {
            Order = order;
            DataContext = Order;
            isEditMode = true;

            dtpOrderDate.SelectedDate = order.OrderDate;
            txtOrderTime.Text = order.OrderTime.ToString(@"hh\:mm\:ss");
            txtCustomerName.Text = order.CustomerName;
            txtCustomerPhone.Text = order.CustomerPhone;
            txtTotalCost.Text = order.TotalCost.ToString("F2");

            // Задаем выбранное значение для ComboBox
            foreach (ComboBoxItem item in cmbCompletionStatus.Items)
            {
                if (item.Content.ToString() == order.CompletionStatus)
                {
                    cmbCompletionStatus.SelectedItem = item;
                    break;
                }
            }
            cmbEmployee.SelectedValue = order.EmployeeId;
        }


        private void LoadEmployees()
        {
            employees.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT employee_id, full_name FROM employees", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    employees.Add(new Employee
                    {
                        EmployeeId = (int)reader["employee_id"],
                        FullName = reader["full_name"].ToString()
                    });
                }
            }
            cmbEmployee.ItemsSource = employees;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Order.OrderDate = dtpOrderDate.SelectedDate ?? DateTime.Now;
            Order.OrderTime = TimeSpan.TryParse(txtOrderTime.Text, out TimeSpan orderTime) ? orderTime : TimeSpan.Zero;
            Order.CustomerName = txtCustomerName.Text;
            Order.CustomerPhone = txtCustomerPhone.Text;
            Order.TotalCost = decimal.TryParse(txtTotalCost.Text, out decimal totalCost) ? totalCost : 0;
            Order.CompletionStatus = (cmbCompletionStatus.SelectedItem as ComboBoxItem)?.Content.ToString();
            Order.EmployeeId = (int)(cmbEmployee.SelectedValue ?? 0);

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
