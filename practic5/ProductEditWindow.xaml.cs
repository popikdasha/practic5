using System;
using System.Linq;
using System.Windows;

namespace practic5
{
    public partial class ProductEditWindow : Window
    {
        public string ProductName { get; private set; }
        public decimal Price { get; private set; }
        public int? CategoryId { get; private set; }

        public ProductEditWindow(ShopDbDataSet shopDbDataSet, string productName = "", decimal price = 0, int? categoryId = null)
        {
            InitializeComponent();

            // Заполняем ComboBox категориями
            CategoryComboBox.ItemsSource = shopDbDataSet.Categories;
            CategoryComboBox.SelectedValuePath = "CategoryId";
            CategoryComboBox.DisplayMemberPath = "CategoryName";

            // Устанавливаем начальные значения
            ProductNameTextBox.Text = productName;
            PriceTextBox.Text = price.ToString("0.00");

            if (categoryId.HasValue)
            {
                CategoryComboBox.SelectedValue = categoryId.Value;
            }
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Валидация данных
            if (string.IsNullOrWhiteSpace(ProductNameTextBox.Text))
            {
                MessageBox.Show("Название продукта не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Цена должна быть положительным числом", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ProductName = ProductNameTextBox.Text.Trim();
            Price = price;
            CategoryId = (int)CategoryComboBox.SelectedValue;

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}