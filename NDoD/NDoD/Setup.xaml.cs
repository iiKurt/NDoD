using System;
using System.Windows;
using System.Windows.Controls;

namespace NDoD
{
    /// <summary>
    /// Interaction logic for Setup.xaml
    /// </summary>
    public partial class Setup : Window
    {
        public Setup()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Window window = new MainWindow(int.Parse(CaseCountTextBox.Text));
            this.Close();
            window.Show();
        }

        private void CaseCountTextBox_LostFocus(object sender, EventArgs e)
        {
            int result;
            if (!(int.TryParse(((TextBox)sender).Text, out result) && result >= 5 && result <= 25)) //Isn't a number or it's not in range
            {
                MessageBox.Show("Invalid number of cases. Please enter a numeric value between 5-25.", "Error");
                ((TextBox)sender).Clear();
            }
        }
    }
}
