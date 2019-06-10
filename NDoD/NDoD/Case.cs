using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDoD
{
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
                switch (rng.Next(1, 4)) //Chance of picking either equation type
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

            while (answer % firstNumber != 0) //Make sure there aren't and decimals involved
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
