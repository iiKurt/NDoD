﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;

namespace NDoD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool holdingCase = false; //Stores whether the user has picked a case to hold
        ObservableCollection<Case> AvailableCases = new ObservableCollection<Case>();
        ObservableCollection<Case> ClaimedCases = new ObservableCollection<Case>();

        int _waitingCases;
        int WaitingCases //Number of cases needed to be opened until the banker will be summoned
        { //TODO: bind this instead of using property
            get
            {
                return _waitingCases;
            }
            set
            {
                _waitingCases = value;
                BankerStatusLabel.Content = "Open " + value + " more cases to summon the BANKER!";
            }
        }
        int CaseCount; //Stores the user's selection for number of cases to generate
        bool WonGame; //Game has been won

        public MainWindow(int caseCount, bool mathMode)
        {
            InitializeComponent();

            CaseCount = caseCount; //Store the variable in a way that other functions can access it
            WaitingCases = caseCount / 3; //The number of cases needed to initally summon the banker varies depending on how many cases are actually available

            AddButtons(CaseCount); //Generate buttons on the grid that let the user pick a case
            GenerateCases(mathMode); //Make some new cases and add then to the list

            TutorialLabel.Content = "First, select a case to hold."; //Set the label on the window
        }

        void AddButtons(int caseCount) //Adds all the case buttons to the grid
        {
            int marginLeft = 150;
            int marginTop = 61;
            const int marginLeftIncrement = 80;
            const int marginTopIncrement = 25;

            for (int i = 0; i < caseCount; i++)
            {
                Button button = new Button() //Initalize each button
                {
                    Content = i + 1, //Starts at zero, sets the label
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 60,
                    Margin = new Thickness(marginLeft, marginTop, 0, 0)
                };
                button.Click += CaseButton_Click; //All the buttons call the same event, but a random case is chosen within the event

                WindowGrid.Children.Add(button); //Actually add the button to the GUI

                marginLeft += marginLeftIncrement;
                if (marginLeft >= 460) //After this the buttons go off the screen
                {
                    marginLeft -= marginLeftIncrement * 4; //Limit of buttons horizontally
                    marginTop += marginTopIncrement;
                }
            }
        }

        void GenerateCases(bool mathMode)
        {
            //Add some new cases to the list, for the user to open
            if (mathMode)
            {
                for (int i = 0; i < CaseCount; i++)
                {
                    Case newCase = new Case();
                    newCase.AssignEquation();
                    AvailableCases.Add(newCase); //Add it to the list
                }
            }
            else
            {
                for (int i = 0; i < CaseCount; i++)
                {
                    AvailableCases.Add(new Case()); //Add it to the list
                }
            }

            AvailableCasesListBox.ItemsSource = AvailableCases; //Bind the AvailableCases list to the listbox on the window
            ClaimedCasesListBox.ItemsSource = ClaimedCases; //Bind the ClaimedCases list to the other listbix on the window
        }
        
        private void CaseButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false; //Disables the button so the user can't click on the same case more than once

            if (holdingCase) //Only allow opening a case when they have already selected a case to hold
            {
                OpenCase(sender as Button);
            }
            else
            {
                PickupCase(sender as Button);
            }
        }

        void OpenCase(Button toOpen)
        {
            Random rng = new Random();
            int randomCase = rng.Next(AvailableCases.Count);
            
            MessageBox.Show("This case contains: $" + AvailableCases[randomCase].Question);
            ClaimCase(randomCase);

            DecideOnSummoningBanker();

            if (AvailableCases.Count <= 2) //Less than 3 cases are left
            {
                WinGame();
            }

            TutorialLabel.Content = "Finally, continue opening cases for a better deal!";
        }

        void PickupCase(Button toPickup)
        {
            //Hide the case that has been choosen
            HoldingCaseButton.Content = toPickup.Content;
            HoldingCaseButton.Visibility = Visibility.Visible;
            toPickup.Visibility = Visibility.Hidden;

            holdingCase = true; //The user is now holding a case

            TutorialLabel.Content = "Next, select a case to open.";
        }

        void ClaimCase(int index) //Move case from AvailableCases -> ClaimedCases
        {
            ClaimedCases.Add(AvailableCases[index]);
            AvailableCases.RemoveAt(index);
        }

        void DecideOnSummoningBanker()
        {
            WaitingCases--; //Decrement the number of cases needed to summon the banker

            //Choose the number of cases to be opened until banker is summeoned
            if (WaitingCases <= 0) //There are no more cases left to open
            {
                //Set some more cases for the user to open
                if (AvailableCases.Count <= CaseCount / 3)
                {
                    WaitingCases = 1;
                }
                else
                {
                    WaitingCases = CaseCount / 4;
                }

                SummonBanker(); //Summon the banker
            }
        }

        void SummonBanker()
        {
            int offer = AverageCaseValue(AvailableCases);

            MessageBoxResult bankerChoice = MessageBox.Show("The banker has been summoned!\n\nThe banker's offer is: $" + String.Format("{0:n0}", offer) + ".\n\nDo you accept?",
                        "Banker's Offer", MessageBoxButton.YesNo);

            if (bankerChoice == MessageBoxResult.Yes) //User accepted offer
            {
                MessageBox.Show("Congratulations! You've won $" + String.Format("{0:n0}", offer) + "!");
                WinGame();
            }
        }

        void WinGame()
        {
            if (!WonGame) //Only call this method once
            {
                MessageBox.Show("Let's see what's in your case...");
                MessageBox.Show("It's $" + String.Format("{0:n0}", AvailableCases[0].Question) + "!!!"); //There will always be a case leftover at index zero

                DialogResult = true; //Tell setup window that the game has ended
                this.Close();

                WonGame = true;
            }
        }

        int AverageCaseValue(ObservableCollection<Case> cases) //Averages all the cases' values
        {
            int totalSum = 0;

            for (int i = 0; i < cases.Count; i++) //Go through each cases' answer
            {
                totalSum += cases[i].Answer; //Add it onto the total sum
            }

            return totalSum / cases.Count; //Return the average
        }
    }
}
