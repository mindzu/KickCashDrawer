using Drawer;
using System;
using System.Threading;
using System.Windows;

namespace AppKickCashDrawer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CashDrawer _cashDrawer;
        public MainWindow()
        {
            InitializeComponent();
            ComPortStatusTextBox.Text = "";
            MessageTextBlock.Text = "";
        }

        private void OpenComPortButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var portName = ComPortTextBox.Text;
                if (_cashDrawer == null)
                {
                    _cashDrawer = new CashDrawer(portName);
                }
                else
                {
                    _cashDrawer.CloseDeviceWithCOMPort();
                    Thread.Sleep(10);
                    _cashDrawer.OpenDeviceWithCOMPort(portName);
                }

                var isComPortOpened = _cashDrawer.ComPort.IsOpen;
                if (isComPortOpened)
                {
                    OpenComPortButton.Content = $"ReOpen {portName}";
                    ComPortStatusTextBox.Text = "OPENED";
                    MessageTextBlock.Text = "";

                    if (CloseComPortButton != null)
                    {
                        CloseComPortButton.Visibility = Visibility.Visible;
                    }

                    if (KickCashDrawerButton != null)
                    {
                        KickCashDrawerButton.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    OpenComPortButton.Content = $"Open COM port";
                    ComPortStatusTextBox.Text = "NOT OPENED";
                    MessageTextBlock.Text = $"Open port {portName} failed!";

                    if (CloseComPortButton != null)
                    {
                        CloseComPortButton.Visibility = Visibility.Collapsed;
                    }

                    if (KickCashDrawerButton != null)
                    {
                        KickCashDrawerButton.Visibility = Visibility.Collapsed;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void KickCashDrawerButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageTextBlock.Text = "";
                if (_cashDrawer != null)
                {
                    var result = _cashDrawer?.OpenCashdrawer();
                    if (result != null && result == true) {
                        var isOpened = _cashDrawer.IsCashdrawerOpening();
                        var message = "Cash drawer status: " + (isOpened ? "OPENNED" : "CLOSED");
                        MessageTextBlock.Text = message;
                    }
                    else
                    {
                        MessageTextBlock.Text = "KICK FAILED";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CloseComPortButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cashDrawer != null)
            {
                _cashDrawer.CloseDeviceWithCOMPort();
                var isComPortOpened = _cashDrawer.ComPort.IsOpen;
                var comPortStatus = isComPortOpened ? "OPENED" : "NOT OPENED";

                CloseComPortButton.Visibility = isComPortOpened ? Visibility.Visible : Visibility.Collapsed;
                KickCashDrawerButton.Visibility = isComPortOpened ? Visibility.Visible : Visibility.Collapsed;
                ComPortStatusTextBox.Text = comPortStatus;
                MessageTextBlock.Text = "";
                OpenComPortButton.Content = $"Open COM port";
            }
        }

        private void ComPortTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            _cashDrawer?.CloseDeviceWithCOMPort();

            if (OpenComPortButton != null)
            {
                OpenComPortButton.Content = $"Open COM port";
            }

            if (ComPortStatusTextBox != null)
            {
                ComPortStatusTextBox.Text = "";
            }

            if (MessageTextBlock != null)
            {
                MessageTextBlock.Text = "";
            }

            if (CloseComPortButton != null)
            {
                CloseComPortButton.Visibility = Visibility.Collapsed;
            }

            if (KickCashDrawerButton != null)
            {
                KickCashDrawerButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
