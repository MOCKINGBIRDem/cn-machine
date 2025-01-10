using System;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using UnifiedApp;

namespace UnifiedApp
{
    public partial class MainWindow : Window
    {
        //private SerialPort _serialPort; // Закомментировано для тестирования без подключения устройства
        private double _positionX = 10;
        private double _positionY = 10;
        private double MovementStep = 1.0;
        private DispatcherTimer moveTimer;

        public MainWindow()
        {
            InitializeComponent();
            //InitializeSerialPort(); // Закомментировано для тестирования без подключения устройства

            moveTimer = new DispatcherTimer();
            moveTimer.Interval = TimeSpan.FromMilliseconds(20); // Таймер обновления
            moveTimer.Tick += MoveTimer_Tick;
            moveTimer.Start();
        }

        //private void InitializeSerialPort()
        //{
        //    try
        //    {
        //        _serialPort = new SerialPort("COM4", 9600);
        //        _serialPort.DataReceived += SerialPort_DataReceived;
        //        _serialPort.Open();
        //        MessageBox.Show("Порт успешно открыт.");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при открытии порта: {ex.Message}");
        //    }
        //}

        //private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    try
        //    {
        //        string data = _serialPort.ReadLine().Trim(); // Получаем данные от Arduino
        //        Application.Current.Dispatcher.Invoke(() =>
        //        {
        //            HandleArduinoInput(data);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show($"Ошибка при обработке данных: {ex.Message}");
        //    }
        //}

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Логика изменения скорости
            if (e.Key == Key.A) // Уменьшение скорости
            {
                if (SpeedSlider.Value > SpeedSlider.Minimum)
                {
                    SpeedSlider.Value -= 5; // Уменьшаем значение скорости
                }
                return; // Завершаем обработку, чтобы не двигать кубик
            }
            else if (e.Key == Key.D) // Увеличение скорости
            {
                if (SpeedSlider.Value < SpeedSlider.Maximum)
                {
                    SpeedSlider.Value += 5; // Увеличиваем значение скорости
                }
                return; // Завершаем обработку, чтобы не двигать кубик
            }

            // Логика движения кубика
            if (e.Key == Key.Left)
            {
                _positionX -= MovementStep; // Движение влево
            }
            else if (e.Key == Key.Right)
            {
                _positionX += MovementStep; // Движение вправо
            }
            else if (e.Key == Key.Up)
            {
                _positionY -= MovementStep; // Движение вверх
            }
            else if (e.Key == Key.Down)
            {
                _positionY += MovementStep; // Движение вниз
            }

            UpdateMoverPosition();
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            // Здесь можно обрабатывать отпускание клавиш, если это необходимо
        }

        private void HandleArduinoInput(string data)
        {
            // {
            // M1:F => передвижение вправо по оси х/M1:B => передвижение влево по оси х, ====> пример отдельной команды для первого шагового двигателя
            // M2:F => передвижение вверх по оси y/M2:B => передвижение вниз по оси y,=======> пример отдельной команды для второго шагового двигателя
            //
            // (примеры вариаций команд для двух шаговых двигателей
            // M1:F M2:F => передвижение вправо по оси х и передвижение вверх по оси y,
            // M1:F M2:B => передвижение вправо по оси х и передвижение вниз по оси y,
            // M1:B M2:F => передвижение влево по оси х и передвижение вверх по оси y,
            // M1:B M2:B => передвижение влево по оси х и передвижение вниз по оси y,
            // )
            // 
            // }
            string[] commands = data.Split(' ');
            foreach (var command in commands)
            {
                if (command.StartsWith("M1:"))
                {
                    string dir = command.Substring(3);
                    if (dir == "F") _positionX += MovementStep;
                    else if (dir == "B") _positionX -= MovementStep;
                }
                else if (command.StartsWith("M2:"))
                {
                    string dir = command.Substring(3);
                    if (dir == "F") _positionY -= MovementStep; // Вверх
                    else if (dir == "B") _positionY += MovementStep; // Вниз
                }
            }
            UpdateMoverPosition();
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            UpdateMoverPosition();
        }


        private void UpdateMoverPosition()
        {
            // Обновляем позицию квадрата на Canvas
            Canvas.SetLeft(Mover, _positionX);
            Canvas.SetTop(Mover, _positionY);
        }

        private void SpeedSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (SpeedTextBox != null)
            {
                SpeedTextBox.Text = ((int)e.NewValue).ToString();
                MovementStep = e.NewValue / 60; // Обновляем шаг движения
            }
        }

        private void AddDetailButton_Click(object sender, RoutedEventArgs e)
        {
            // Создаем диалог для ввода размеров
            var inputDialog = new InputDialog();
            if (inputDialog.ShowDialog() == true)
            {
                // Получаем введенные размеры
                double width = inputDialog.WidthInput;
                double height = inputDialog.HeightInput;

                // Добавляем новый прямоугольник в центр Canvas
                AddDetailToCanvas(width, height);
            }
        }

        private void AddDetailToCanvas(double width, double height)
{
    // Удаляем старые детали, если нужно
    var existingDetail = CanvasArea.Children.OfType<Rectangle>().FirstOrDefault(r => r.Name == "Detail");
    if (existingDetail != null)
    {
        CanvasArea.Children.Remove(existingDetail);
    }

    // Создаем новую деталь
    var detail = new Rectangle
    {
        Name = "Detail",
        Width = width,
        Height = height,
        Fill = Brushes.Red
    };

    // Получаем размеры черной области
    var blackRectangle = CanvasArea.Children.OfType<Rectangle>()
                                    .FirstOrDefault(r => r.Fill == Brushes.Black);  // Проверяем, что это черная область
    if (blackRectangle != null)
    {
        double blackWidth = blackRectangle.Width;
        double blackHeight = blackRectangle.Height;

        // Вычисляем центральное положение относительно черной области
        double centerX = (blackWidth - width) / 2;
        double centerY = (blackHeight - height) / 2;

        // Устанавливаем позицию для детали внутри черной области
        Canvas.SetLeft(detail, centerX);
        Canvas.SetTop(detail, centerY);

        // Добавляем деталь на Canvas
        CanvasArea.Children.Add(detail);
    }
}




        private void SpeedTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(SpeedTextBox.Text, out double speed))
            {
                if (speed >= 60 && speed <= 540)
                {
                    SpeedSlider.Value = speed;
                    MovementStep = speed / 60;

                    // Возвращаем фокус на окно
                    this.Focus();
                }
            }
        }

        private void SetSpeedButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(SpeedTextBox.Text, out double speed))
            {
                if (speed >= 60 && speed <= 540)
                {
                    MovementStep = speed / 60;
                    MessageBox.Show($"Скорость установлена на {speed}");

                    // Возвращаем фокус на окно
                    this.Focus();
                }
                else
                {
                    MessageBox.Show("Скорость должна быть от 60 до 540.");
                }
            }
            else
            {
                MessageBox.Show("Введите корректное значение скорости.");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            // Закрытие порта закомментировано для тестирования
            //if (_serialPort != null && _serialPort.IsOpen)
            //{
            //    _serialPort.Close();
            //}
            base.OnClosed(e);
        }
    }
}
