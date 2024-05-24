using CafeBakery.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace CafeBakery.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для MenuManagementWindow.xaml
    /// </summary>
    public partial class MenuManagementWindow : Window
    {
        public Model.Menu Menu { get; private set; }
        private bool isEditMode;
        private string connectionString = DatabaseConfig.ConnectionString;

        public MenuManagementWindow()
        {
            InitializeComponent();
            Menu = new Model.Menu();
            DataContext = Menu;
            isEditMode = false;
        }

        public MenuManagementWindow(Model.Menu menu) : this()
        {
            Menu = menu;
            DataContext = Menu;
            isEditMode = true;

            txtDishName.Text = menu.DishName;
            txtCost.Text = menu.Cost.ToString();
            txtPreparationTime.Text = menu.PreparationTime.ToString(@"hh\:mm\:ss");
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Menu.DishName = txtDishName.Text;
            Menu.Cost = decimal.TryParse(txtCost.Text, out decimal cost) ? cost : 0;

            if (TimeSpan.TryParse(txtPreparationTime.Text, out TimeSpan preparationTime))
            {
                if (preparationTime.TotalHours >= 24)
                {
                    MessageBox.Show("Время приготовления не может превышать 23:59:59.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                Menu.PreparationTime = preparationTime;
            }
            else
            {
                Menu.PreparationTime = TimeSpan.Zero;
            }

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

    }
}
