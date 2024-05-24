using CafeBakery.Model;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ManageEmployeesWindow.xaml
    /// </summary>
    public partial class ManageEmployeesWindow : Window
    {
        public Employee Employee { get; private set; }
        private bool isEditMode;
        private string connectionString = DatabaseConfig.ConnectionString;
        private List<Position> positions = new List<Position>();
        public ManageEmployeesWindow()
        {
            InitializeComponent();
            Employee = new Employee();
            DataContext = Employee;
            isEditMode = false;
            LoadPositions();
        }

        public ManageEmployeesWindow(Employee employee) : this()
        {
            Employee = employee;
            DataContext = Employee;
            isEditMode = true;
            if (employee.Gender == "Мужской")
            {
                cmbGender.SelectedIndex = 0;
            }
            else
            {
                cmbGender.SelectedIndex = 1;
            }
            cmbPosition.SelectedValue = employee.PositionId;

            txtFullName.Text = employee.FullName;
            txtAge.Text = employee.Age.ToString();
            txtAddress.Text = employee.Address;
            txtPhone.Text = employee.Phone;
            txtPassportDetails.Text = employee.PassportDetails;
        }

        private void LoadPositions()
        {
            positions.Clear();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand("SELECT position_id, position_name FROM positions", connection);
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    positions.Add(new Position
                    {
                        PositionId = (int)reader["position_id"],
                        PositionName = reader["position_name"].ToString()
                    });
                }
            }
            cmbPosition.ItemsSource = positions;
        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (cmbGender.SelectedItem is ComboBoxItem selectedGender)
            {
                Employee.Gender = selectedGender.Content.ToString();
            }

            if (cmbPosition.SelectedValue != null)
            {
                Employee.PositionId = (int)cmbPosition.SelectedValue;
            }

            Employee.FullName = txtFullName.Text;
            Employee.Age = int.TryParse(txtAge.Text, out int age) ? age : 0;
            Employee.Address = txtAddress.Text;
            Employee.Phone = txtPhone.Text;
            Employee.PassportDetails = txtPassportDetails.Text;

            DialogResult = true;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
