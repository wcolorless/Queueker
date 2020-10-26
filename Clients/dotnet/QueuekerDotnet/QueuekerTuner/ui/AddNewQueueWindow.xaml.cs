using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Queueker.core.Protocol;

namespace QueuekerTuner.ui
{
    /// <summary>
    /// Логика взаимодействия для AddNewQueueWindow.xaml
    /// </summary>
    public partial class AddNewQueueWindow : Window
    {
        public AddNewQueueWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void UnlimitedCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (UnlimitedCheckBox.IsChecked == true)
            {
                QueueLimitBox.IsEnabled = false;
            }
        }

        private void UnlimitedCheckBox_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (UnlimitedCheckBox.IsChecked == false)
            {
                QueueLimitBox.IsEnabled = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(QueueNameBox.Text) && (!string.IsNullOrEmpty(QueueLimitBox.Text) || UnlimitedCheckBox.IsChecked == true) )
            {
                var newQueue = new QueueItemInfo()
                {
                    Name = QueueNameBox.Text,
                    Limit = UnlimitedCheckBox.IsChecked == true ? -1L : long.Parse(QueueLimitBox.Text)
                };
                DataContext = newQueue;
                Close();
            }
        }
    }
}
