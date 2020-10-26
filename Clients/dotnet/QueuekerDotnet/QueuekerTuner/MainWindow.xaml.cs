using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using QueuekerTuner.core;
using QueuekerTuner.ui.logic;

namespace QueuekerTuner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PrimeWindowVM _primeWindowVm;
        public MainWindow()
        {
            _primeWindowVm = PrimeWindowVM.Create(this, Thread.CurrentThread);
            InitializeComponent();
            DataContext = _primeWindowVm;
            
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void TextBlock_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InputHost.Text) && !string.IsNullOrEmpty(InputLogin.Text) && !string.IsNullOrEmpty(InputPassword.Text))
            {
                Credentials.Host = InputHost.Text;
                Credentials.Login = InputLogin.Text;
                Credentials.Password = InputPassword.Text;
            }
        }

        private async void ChangeLoginAndPassButtonClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(NewLoginBox.Text) && !string.IsNullOrEmpty(NewPassBox.Text) && !string.IsNullOrEmpty(NewPassTwinBox.Text))
            {
                if (string.Equals(NewPassBox.Text, NewPassTwinBox.Text))
                {
                    Credentials.NewLogin = NewLoginBox.Text;
                    Credentials.NewPassword = NewPassBox.Text;
                    await _primeWindowVm.ChangeLoginAndPasswordCommand(NewLoginBox.Text, NewPassBox.Text);
                }
            }
        }
    }
}
