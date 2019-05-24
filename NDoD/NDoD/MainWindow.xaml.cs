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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace NDoD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool holdingCase = false;
        ObservableCollection<Case> AvailableCases = new ObservableCollection<Case>();
        ObservableCollection<Case> ClaimedCases = new ObservableCollection<Case>();

        public MainWindow()
        {
            InitializeComponent();

            MessageBox.Show("First, select a case to hold.");

            for (int i = 0; i < 6; i++)
            {
                AvailableCases.Add(new Case());
            }

            AvailableCasesListBox.ItemsSource = AvailableCases;
            ClaimedCasesListBox.ItemsSource = ClaimedCases;
        }

        private void CaseButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false;

            if (holdingCase)
            {
                Random rng = new Random();
                int randomCase = rng.Next(AvailableCases.Count);

                MessageBox.Show("This case contains: $" + AvailableCases[randomCase].Question);
                ClaimCase(randomCase);
            }
            else
            {
                SelectedCaseButton.Content = (sender as Button).Content;
                SelectedCaseButton.Visibility = Visibility.Visible;
                (sender as Button).Visibility = Visibility.Hidden;

                holdingCase = true;

                MessageBox.Show("Next, select a case to open.");
            }
        }

        void ClaimCase(int index)
        {
            Case swap = AvailableCases[index];
            AvailableCases.RemoveAt(index);
            ClaimedCases.Add(swap);
        }

        class Case
        {
            public string Question { get; private set; }
            public int Answer { get; private set; }

            static Random rng = new Random();

            public Case()
            {
                New();
            }

            void New()
            {
                int firstNumber = rng.Next(1, 100);
                int secondNumber = rng.Next(1, 100);

                Question = firstNumber + " + " + secondNumber;
                Answer = firstNumber + secondNumber;
            }
        }
    }
}
