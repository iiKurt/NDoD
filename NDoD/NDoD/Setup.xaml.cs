using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NDoD
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        int CaseCount = 25;

        public Setup()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new MainWindow(CaseCount);
            window.Show();
            this.Close();
        }

        private void CaseCountTextBox_Preview(object sender, TextCompositionEventArgs e) //TODO: Fix this
        {
            if (char.IsDigit(e.Text, e.Text.Length - 1))
            {
                CaseCount = int.Parse(e.Text);
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
