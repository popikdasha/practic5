using System;
using System.Data;
using System.Linq;
using System.Windows;

namespace practic5
{
    public partial class CategoriesWindow : Window
    {
        private ShopDbDataSet shopDbDataSet;
        private ShopDbDataSetTableAdapters.CategoriesTableAdapter categoriesTableAdapter;

        public CategoriesWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            shopDbDataSet = new ShopDbDataSet();
            categoriesTableAdapter = new ShopDbDataSetTableAdapters.CategoriesTableAdapter();
            categoriesTableAdapter.Fill(shopDbDataSet.Categories);
            CategoriesGrid.ItemsSource = shopDbDataSet.Categories.DefaultView;
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            CategoryEditWindow editWindow = new CategoryEditWindow();
            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    categoriesTableAdapter.Insert(editWindow.CategoryName);
                    shopDbDataSet.Categories.Clear();
                    categoriesTableAdapter.Fill(shopDbDataSet.Categories);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при добавлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void EditCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию для редактирования", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)CategoriesGrid.SelectedItem;
            CategoryEditWindow editWindow = new CategoryEditWindow(row["CategoryName"].ToString());

            if (editWindow.ShowDialog() == true)
            {
                try
                {
                    row["CategoryName"] = editWindow.CategoryName;
                    categoriesTableAdapter.Update(shopDbDataSet.Categories);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при обновлении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesGrid.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию для удаления", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DataRowView row = (DataRowView)CategoriesGrid.SelectedItem;
            int categoryId = (int)row["CategoryId"];

            try
            {
                // Проверяем, есть ли продукты в этой категории
                var productsTableAdapter = new ShopDbDataSetTableAdapters.ProductsTableAdapter();
                var products = productsTableAdapter.GetData().Where(p => p.CategoryId == categoryId);

                if (products.Any())
                {
                    MessageBox.Show("Нельзя удалить категорию, так как существуют продукты, относящиеся к ней.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (MessageBox.Show("Вы уверены, что хотите удалить эту категорию?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    row.Delete();
                    categoriesTableAdapter.Update(shopDbDataSet.Categories);
                    shopDbDataSet.Categories.Clear();
                    categoriesTableAdapter.Fill(shopDbDataSet.Categories);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении категории: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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