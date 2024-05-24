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
    /// <summary>
    /// Логика взаимодействия для ManageEmployees.xaml
    /// </summary>
    public partial class EmployeesListPage : Page
    {
        private string connectionString = DatabaseConfig.ConnectionString;
        private List<Employee> employees = new List<Employee>();
        public EmployeesListPage()
        {
            InitializeComponent();
            LoadEmployees();
        }

        private void LoadEmployees()
        {
            employees.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT employee_id, full_name, age, gender, address, phone, passport_details FROM employees", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var employee = new Employee
                    {
                        EmployeeId = (int)reader["employee_id"],
                        FullName = reader["full_name"].ToString(),
                        Age = (int)reader["age"],
                        Gender = reader["gender"].ToString(),
                        Address = reader["address"].ToString(),
                        Phone = reader["phone"].ToString(),
                        PassportDetails = reader["passport_details"].ToString()
                    };
                    employees.Add(employee);
                }
            }
            listViewEmployees.ItemsSource = null;
            listViewEmployees.ItemsSource = employees;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchBox.Text.ToLower();
            var filteredEmployees = employees.Where(emp => emp.FullName.ToLower().Contains(searchText)).ToList();
            listViewEmployees.ItemsSource = filteredEmployees;
        }

        private void listViewEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new ManageEmployeesWindow();
            if (dialog.ShowDialog() == true)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand(
                        "INSERT INTO employees (full_name, age, gender, address, phone, passport_details, position_id) VALUES (@full_name, @age, @gender, @address, @phone, @passport_details, @position_id)",
                        connection);
                    command.Parameters.AddWithValue("@full_name", dialog.Employee.FullName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@age", dialog.Employee.Age);
                    command.Parameters.AddWithValue("@gender", dialog.Employee.Gender ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@address", dialog.Employee.Address ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@phone", dialog.Employee.Phone ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@passport_details", dialog.Employee.PassportDetails ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@position_id", dialog.Employee.PositionId);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadEmployees();
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (listViewEmployees.SelectedItem is Employee selectedEmployee)
            {
                var dialog = new ManageEmployeesWindow(selectedEmployee);
                if (dialog.ShowDialog() == true)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        SqlCommand command = new SqlCommand(
                            "UPDATE employees SET full_name = @full_name, age = @age, gender = @gender, address = @address, phone = @phone, passport_details = @passport_details, position_id = @position_id WHERE employee_id = @employee_id",
                            connection);
                        command.Parameters.AddWithValue("@employee_id", selectedEmployee.EmployeeId);
                        command.Parameters.AddWithValue("@full_name", dialog.Employee.FullName ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@age", dialog.Employee.Age);
                        command.Parameters.AddWithValue("@gender", dialog.Employee.Gender ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@address", dialog.Employee.Address ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@phone", dialog.Employee.Phone ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@passport_details", dialog.Employee.PassportDetails ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@position_id", dialog.Employee.PositionId);

                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    LoadEmployees();
                }
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (listViewEmployees.SelectedItem is Employee selectedEmployee)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    SqlCommand command = new SqlCommand("DELETE FROM employees WHERE employee_id = @employee_id", connection);
                    command.Parameters.AddWithValue("@employee_id", selectedEmployee.EmployeeId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                LoadEmployees();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
