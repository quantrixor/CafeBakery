using CafeBakery.Model;
using System;
using System.Windows;

namespace CafeBakery.Views.Windows
{
    public partial class InventoryManagementWindow : Window
    {
        public Inventory Inventory { get; private set; }
        private bool isEditMode;

        public InventoryManagementWindow()
        {
            InitializeComponent();
            Inventory = new Inventory();
            DataContext = Inventory;
            isEditMode = false;
        }

        public InventoryManagementWindow(Inventory inventory) : this()
        {
            Inventory = inventory;
            DataContext = Inventory;
            isEditMode = true;

            txtIngredientName.Text = inventory.IngredientName;
            dtpProductionDate.SelectedDate = inventory.ProductionDate;
            txtVolume.Text = inventory.Volume.ToString("F2");
            dtpExpiryDate.SelectedDate = inventory.ExpiryDate;
            txtCost.Text = inventory.Cost.ToString("F2");
            txtSupplier.Text = inventory.Supplier;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Inventory.IngredientName = txtIngredientName.Text;
            Inventory.ProductionDate = dtpProductionDate.SelectedDate ?? DateTime.Now;
            Inventory.Volume = decimal.TryParse(txtVolume.Text, out decimal volume) ? volume : 0;
            Inventory.ExpiryDate = dtpExpiryDate.SelectedDate ?? DateTime.Now;
            Inventory.Cost = decimal.TryParse(txtCost.Text, out decimal cost) ? cost : 0;
            Inventory.Supplier = txtSupplier.Text;

            DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
