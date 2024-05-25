using CafeBakery.Model;
using System;
using System.Windows;

namespace CafeBakery.Views.Windows
{
    public partial class PositionManagementWindow : Window
    {
        public Position Position { get; private set; }
        private bool isEditMode;

        public PositionManagementWindow()
        {
            InitializeComponent();
            Position = new Position();
            DataContext = Position;
            isEditMode = false;
        }

        public PositionManagementWindow(Position position) : this()
        {
            Position = position;
            DataContext = Position;
            isEditMode = true;

            txtPositionName.Text = position.PositionName;
            txtSalary.Text = position.Salary.ToString("F2");
            txtResponsibilities.Text = position.Responsibilities;
            txtRequirements.Text = position.Requirements;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Position.PositionName = txtPositionName.Text;
            Position.Salary = decimal.TryParse(txtSalary.Text, out decimal salary) ? salary : 0;
            Position.Responsibilities = txtResponsibilities.Text;
            Position.Requirements = txtRequirements.Text;

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
