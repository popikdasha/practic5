using System.Windows;

namespace practic5
{
    public partial class CategoryEditWindow : Window
    {
        public string CategoryName { get; private set; }

        public CategoryEditWindow(string categoryName = "")
        {
            InitializeComponent();
            CategoryNameTextBox.Text = categoryName;
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                MessageBox.Show("Название категории не может быть пустым", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CategoryName = CategoryNameTextBox.Text.Trim();
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