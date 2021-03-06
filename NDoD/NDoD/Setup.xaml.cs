﻿using System.Windows;

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
            if (ValidateCaseCountTextBox())
            {
                Case.AllRewards = new Reward(); //Reset rewards

                bool mathMode = MathModeCheckBox.IsChecked ?? true; //If null, assume we want MathMode

                Window window = new MainWindow(int.Parse(CaseCountTextBox.Text), mathMode);
                this.Hide();

                if (window.ShowDialog() == true) //Game has ended without quitting
                {
                    if (MessageBox.Show("Do you want to play again?", "Play again?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    { //User selected yes
                        this.Show();
                    }
                    else
                    { //User selected no
                        this.Close();
                    }
                }
                else
                { //Game was quit
                    this.Close();
                }
            }
        }

        private bool ValidateCaseCountTextBox()
        {
            int result;
            if (!(int.TryParse(CaseCountTextBox.Text, out result) && result >= 5 && result <= 25)) //Isn't a number or it's not in range
            {
                MessageBox.Show("Invalid number of cases. Please enter a numeric value between 5-25.", "Error");
                return false;
            }
            return true;
        }

        private void InstructionsButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("No Deal or Deal puts YOU in the hotseat to win the most amount of money possible.\n" +
                "After picking your case, you need to open enough cases until the banker offers to buy out your case for an average of all the available cases.\n" +
                "This keeps on repeating, unless you keep denying all of the offers, until two cases are left, when then, you finally get to open your case. You win whatever is in your case.",
                "Instructions");
        }
    }
}
