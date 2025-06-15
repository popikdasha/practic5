using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace practic5
{
    public partial class ProductsWindow : Window
    {
        private ShopDbDataSet shopDbDataSet;
        private ShopDbDataSetTableAdapters.ProductsTableAdapter productsTableAdapter;
        private ShopDbDataSetTableAdapters.CategoriesTableAdapter categoriesTableAdapter;

        public ProductsWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            shopDbDataSet = new ShopDbDataSet();
            productsTableAdapter = new ShopDbDataSetTableAdapters.ProductsTableAdapter();
            categoriesTableAdapter = new ShopDbDataSetTableAdapters.CategoriesTableAdapter();

            productsTableAdapter.Fill(shopDbDataSet.Products);
            categoriesTableAdapter.Fill(shopDbDataSet.Categories);

                        if (!shopDbDataSet.Products.Columns.Contains("CategoryName"))
            {
                                string relationName = FindRelationName();

                                shopDbDataSet.Products.Columns.Add(
                    "CategoryName",
                    typeof(string),
                    $"Parent({relationName}).CategoryName");
            }

            ProductsGrid.ItemsSource = shopDbDataSet.Products.DefaultView;
        }

        private string FindRelationName()
        {
                        foreach (DataRelation relation in shopDbDataSet.Relations)
            {
                if (relation.ParentTable == shopDbDataSet.Categories &&
                    relation.ChildTable == shopDbDataSet.Products)
                {
                    return relation.RelationName;
                }
            }
            throw new Exception("Не найдено отношение между Categories и Products");
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductEditWindow editWindow = new ProductEditWindow(shopDbDataSet);
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    productsTableAdapter.Insert(
                        editWindow.ProductName,
                        editWindow.Price,
                        editWindow.CategoryId.Value);

                    shopDbDataSet.Products.Clear();
                    productsTableAdapter.Fill(shopDbDataSet.Products);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)ProductsGrid.SelectedItem;
            ProductEditWindow editWindow = new ProductEditWindow(
                shopDbDataSet,
                row["ProductName"].ToString(),
                (decimal)row["Price"],
                row["CategoryId"] as int?);

            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    row["ProductName"] = editWindow.ProductName;
                    row["Price"] = editWindow.Price;
                    row["CategoryId"] = editWindow.CategoryId;

                    productsTableAdapter.Update(shopDbDataSet.Products);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите продукт для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)ProductsGrid.SelectedItem;

            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить этот продукт?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    row.Delete();
                    productsTableAdapter.Update(shopDbDataSet.Products);
                    shopDbDataSet.Products.Clear();
                    productsTableAdapter.Fill(shopDbDataSet.Products);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении продукта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}