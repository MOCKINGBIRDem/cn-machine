using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace UnifiedApp
{

    public partial class MainWindow : Window
    {
        private double _positionX = 0; // Начальная координата X
        private double _positionY = 0; // Начальная координата Y
        private double _targetX = 0; // Целевая координата X
        private double _targetY = 0; // Целевая координата Y
        private double _step = 0.1; // Шаг перемещения
        private DispatcherTimer _moveTimer; // Таймер для обновления позиции
        private double _moveSpeed = 100; // Скорость перемещения
        private const double TimerInterval = 0.1; // Интервал обновления таймера

        private double _point1X, _point1Y; // Координаты первой точки
        private double _point2X, _point2Y; // Координаты второй точки
        private bool _movingToFirstPoint = true; // Флаг для отслеживания направления движения
        private int _cyclesCount = 0; // Счетчик циклов
        private int _totalCycles = 0; // Общее количество циклов

        public MainWindow()
        {
            InitializeComponent();

            _moveTimer = new DispatcherTimer();
            _moveTimer.Interval = TimeSpan.FromMilliseconds(TimerInterval);
            _moveTimer.Tick += MoveTimer_Tick;

            Canvas.SetLeft(Burner, _positionX);
            Canvas.SetTop(Burner, _positionY);
        }

        private void StartMovementButton_Click(object sender, RoutedEventArgs e)
        {
            // Чтение координат и количества циклов
            if (double.TryParse(X1CoordinateTextBox.Text, out double point1X) &&
                double.TryParse(Y1CoordinateTextBox.Text, out double point1Y) &&
                double.TryParse(X2CoordinateTextBox.Text, out double point2X) &&
                double.TryParse(Y2CoordinateTextBox.Text, out double point2Y) &&
                int.TryParse(CyclesTextBox.Text, out int totalCycles))
            {
                _point1X = point1X;
                _point1Y = point1Y;
                _point2X = point2X;
                _point2Y = point2Y;
                _totalCycles = totalCycles;

                _targetX = _point1X;
                _targetY = _point1Y;
                _movingToFirstPoint = true;
                _cyclesCount = 0; // Сброс счетчика циклов

                _moveTimer.Start();
            }
            else
            {
                MessageBox.Show("Please enter valid numeric coordinates and number of cycles.");
            }
        }

        private void StopMovementButton_Click(object sender, RoutedEventArgs e)
        {
            _moveTimer.Stop();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            double deltaX = _targetX - _positionX;
            double deltaY = _targetY - _positionY;

            // Если еще не достигли цели
            if (Math.Abs(deltaX) > _step || Math.Abs(deltaY) > _step)
            {
                _positionX += _step * Math.Sign(deltaX);
                _positionY += _step * Math.Sign(deltaY);

                Canvas.SetLeft(Burner, _positionX);
                Canvas.SetTop(Burner, _positionY);
            }
            else
            {
                // Если достигли точки, сменить цель
                if (_movingToFirstPoint)
                {
                    _targetX = _point2X;
                    _targetY = _point2Y;
                    _movingToFirstPoint = false;
                }
                else
                {
                    _targetX = _point1X;
                    _targetY = _point1Y;
                    _movingToFirstPoint = true;
                    _cyclesCount++;

                    // Если выполнены все циклы, остановить движение
                    if (_cyclesCount >= _totalCycles)
                    {
                        _moveTimer.Stop();
                        MessageBox.Show("Movement complete!");
                    }
                }
            }
        }

        private void CoordinateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && (textBox.Text == "Enter X1" || textBox.Text == "Enter Y1" || textBox.Text == "Enter X2" || textBox.Text == "Enter Y2"))
            {
                textBox.Text = string.Empty;
                textBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void CoordinateTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                // Возврат текста-подсказки, если поле пустое
                if (textBox == X1CoordinateTextBox) textBox.Text = "Enter X1";
                if (textBox == Y1CoordinateTextBox) textBox.Text = "Enter Y1";
                if (textBox == X2CoordinateTextBox) textBox.Text = "Enter X2";
                if (textBox == Y2CoordinateTextBox) textBox.Text = "Enter Y2";
                textBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
    }
}
