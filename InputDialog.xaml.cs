using System.Windows;

namespace UnifiedApp
{
    public partial class InputDialog : Window
    {
        public double WidthInput { get; private set; }
        public double HeightInput { get; private set; }

        public InputDialog()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(WidthBox.Text, out double width) &&
                double.TryParse(HeightBox.Text, out double height))
            {
                WidthInput = width;
                HeightInput = height;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Введите корректные размеры.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
