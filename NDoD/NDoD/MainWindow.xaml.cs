using System;
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
        bool holdingCase = false;
        ObservableCollection<Case> AvailableCases = new ObservableCollection<Case>();
        ObservableCollection<Case> ClaimedCases = new ObservableCollection<Case>();

        int _waitingCases = 8;
        int waitingCases
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

        public MainWindow(int caseCount)
        {
            InitializeComponent();

            AddButtons(caseCount);

            //Add some new cases for the user to open
            for (int i = 0; i < caseCount; i++)
            {
                AvailableCases.Add(new Case());
            }

            AvailableCasesListBox.ItemsSource = AvailableCases;
            ClaimedCasesListBox.ItemsSource = ClaimedCases;

            TutorialLabel.Content = "First, select a case to hold.";
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
                button.Click += CaseButton_Click;

                WindowGrid.Children.Add(button); //Actually add the button to the GUI

                marginLeft += marginLeftIncrement;
                if (marginLeft >= 460) //After this the buttons go off the screen
                {
                    marginLeft -= marginLeftIncrement * 4; //Limit of buttons horizontally
                    marginTop += marginTopIncrement;
                }
            }
        }
        
        private void CaseButton_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).IsEnabled = false; //Disables the button so the user can't click on the same case more than once

            if (holdingCase) //Only allow opening a case when they have already selected a case to hold
            {
                Random rng = new Random();
                int randomCase = rng.Next(AvailableCases.Count);

                MessageBox.Show("This case contains: $" + AvailableCases[randomCase].Question + " Answer: " + AvailableCases[randomCase].Answer);
                ClaimCase(randomCase);

                waitingCases--;

                //Choose the number of cases to be opened until banker is summeoned
                if (waitingCases <= 0)
                {
                    if (AvailableCases.Count <= 8)
                    {
                        waitingCases = 1;
                    }
                    else
                    {
                        waitingCases = 6;
                    }

                    SummonBanker();
                }

                if (AvailableCases.Count <= 2)
                {
                    MessageBox.Show("Let's see what's in your case...");
                    MessageBox.Show("It's $" + AvailableCases[0].Question + "!!!");
                    this.Close();
                }

                TutorialLabel.Content = "Finally, continue opening cases for a better deal!";
            }
            else
            {
                //Hide the case that has been choosen
                SelectedCaseButton.Content = (sender as Button).Content;
                SelectedCaseButton.Visibility = Visibility.Visible;
                (sender as Button).Visibility = Visibility.Hidden;

                holdingCase = true;

                TutorialLabel.Content = "Next, select a case to open.";
            }
        }

        void SummonBanker()
        {
            int offer = AverageCaseValue(AvailableCases);

            MessageBoxResult bankerChoice = MessageBox.Show("The banker has been summoned!\n\nHe offers $" + offer + ", do you accept?",
                        "Banker's Offer", MessageBoxButton.YesNo);

            if (bankerChoice == MessageBoxResult.Yes)
            {
                MessageBox.Show("Congratulations you've won $" + offer + "!");
                this.Close();
            }
        }

        void ClaimCase(int index) //Move case from AvailableCases -> ClaimedCases
        {
            ClaimedCases.Add(AvailableCases[index]);
            AvailableCases.RemoveAt(index);
        }

        int AverageCaseValue(ObservableCollection<Case> cases) //Averages all the cases' values
        {
            int totalSum = 0;

            for (int i = 0; i < cases.Count; i++)
            {
                totalSum += cases[i].Answer;
            }

            return totalSum / cases.Count;
        }
    }
}
