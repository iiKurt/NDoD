using System;
using System.Collections.Generic;
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
        const int CaseCount = 16; //Determines amount of buttons/cases to generate

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

            MessageBox.Show("First, select a case to hold.", "Tutorial");

            for (int i = 0; i < caseCount; i++)
            {
                AvailableCases.Add(new Case());
            }

            AvailableCasesListBox.ItemsSource = AvailableCases;
            ClaimedCasesListBox.ItemsSource = ClaimedCases;
        }

        void AddButtons(int caseCount)
        {
            int marginLeft = 150;
            int marginTop = 61;
            const int marginLeftIncrement = 80;
            const int marginTopIncrement = 25;

            for (int i = 0; i < caseCount; i++)
            {
                Button button = new Button()
                {
                    Content = i + 1, //starts at zero
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Width = 60,
                    Margin = new Thickness(marginLeft, marginTop, 0, 0)
                };
                button.Click += CaseButton_Click;

                WindowGrid.Children.Add(button);

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
            (sender as Button).IsEnabled = false;

            if (holdingCase)
            {
                Random rng = new Random();
                int randomCase = rng.Next(AvailableCases.Count);

                MessageBox.Show("This case contains: $" + AvailableCases[randomCase].Question + " Answer: " + AvailableCases[randomCase].Answer);
                ClaimCase(randomCase);

                waitingCases--;

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
            }
            else
            {
                SelectedCaseButton.Content = (sender as Button).Content;
                SelectedCaseButton.Visibility = Visibility.Visible;
                (sender as Button).Visibility = Visibility.Hidden;

                holdingCase = true;

                MessageBox.Show("Next, select a case to open.", "Tutorial");
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

        void ClaimCase(int index)
        {
            Case swap = AvailableCases[index];
            AvailableCases.RemoveAt(index);
            ClaimedCases.Add(swap);
        }

        int AverageCaseValue(ObservableCollection<Case> cases)
        {
            int totalSum = 0;

            for (int i = 0; i < cases.Count; i++)
            {
                totalSum += cases[i].Answer;
            }

            return totalSum / cases.Count;
        }
    }

    class Case
    {
        public string Question { get; private set; }
        public int Answer { get; private set; }

        static List<int> Rewards = new List<int> { 1, 5, 10, 25, 50, 75, 100, 200, 300, 400, 500, 750, 1000, 5000, 10000, 25000, 50000, 75000, 100000, 200000, 300000, 400000, 500000, 750000, 1000000 };
        static Random rng = new Random();

        public Case()
        {
            AssignEquation();
        }

        void AssignEquation()
        {
            int chosenReward = Rewards[rng.Next(0, Rewards.Count - 1)]; //Index is zero based
            Rewards.Remove(chosenReward);

            Answer = chosenReward;

            if (chosenReward >= 5000)
            { //Higher rewards, harder equations
                Question = NewMultiplication(Answer);
            }
            else //Lower rewards, eaiser equations
            {
                switch (rng.Next(1, 4)) //chance of picking either equation type
                {
                    case 1:
                        Question = NewAddition(Answer);
                        break;
                    case 2:
                        Question = NewSubtraction(Answer);
                        break;
                    case 3:
                        Question = NewDivision(Answer);
                        break;
                    default:
                        break;
                }
            }
        }

        string NewAddition(int answer)
        {
            int firstNumber = rng.Next(1, answer);

            return firstNumber + " + " + (answer - firstNumber);
        }

        string NewSubtraction(int answer)
        {
            int firstNumber = rng.Next(1, answer);

            return (answer + firstNumber) + " - " + firstNumber;
        }

        string NewMultiplication(int answer)
        {
            int firstNumber = rng.Next(1, answer);

            while (answer % firstNumber != 0)
            {
                firstNumber = rng.Next(1, answer);
            }

            return (answer / firstNumber) + " * " + firstNumber;
        }

        string NewDivision(int answer)
        {
            int firstNumber = rng.Next(1, answer);

            return (answer * firstNumber) + " / " + firstNumber;
        }
    }
}
