/*
 * Program:     TestLibrary.dll
 * Module:      NumBingoCard.cs
 * Developer:   Dianne Corpuz
 * Date:        April 9, 2021
 * Summary:     Number Bingo Class that generates bingo card, handles user inputs(number) that will be used to match the numbers
 */
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestLibrary
{
    public class NumBingoCard
    {
        private List<uint> userInputs;   //numbers input by user

        //c'tor
        public NumBingoCard()
        {
        }

        //setters - set User inputs
        public void setUserInput(List<uint> inputs)
        {
            this.userInputs = inputs;
        }

        //getters - returns User inputs 
        public List<uint> getUserInputs()
        {
            return this.userInputs;
        }

        //Generate numbers and store it into multimendional array called numBingoCard
        public  uint[,] GenerateNumbBingoCard()
        {
            uint[,] numBingoCard;
            //x and y axis dimension
            uint horizontal = 5;
             uint vertical = 5;
            int count = 0;

            // 2D array
            numBingoCard = new uint[vertical, horizontal];  //initialize dimensions
            List<int> generateNumbers = Enumerable.Range(1, (int)(horizontal * vertical)).ToList();
            generateNumbers = generateNumbers.OrderBy(item => Guid.NewGuid()).ToList(); //shuffle numbers

            //loops 5 times horizontally and vertically, then populate it and store in a multidimensional array
            for (int x = 0; x < vertical; ++x)
            {
                for (int y = 0; y < horizontal; ++y)
                    numBingoCard[x, y] = (uint)generateNumbers[count++];
            }
            return numBingoCard;
        }

        //Display the generated numbers
        public static void NumBingoCardDisplay(uint[,] numBingoCard)
        {
            //x and y axis dimension
            int vertical = numBingoCard.GetUpperBound(0) + 1;
            int horizontal = numBingoCard.GetUpperBound(1) + 1;

            Console.WriteLine("+------------------------+");
            Console.WriteLine("|     My NumBingo Card   |");
            Console.WriteLine("+------------------------+");

            Console.WriteLine("|------------------------|");
            for (int i = 0; i < vertical; ++i)
            {
                Console.Write("|");
                for (int j = 0; j < horizontal; ++j)
                    Console.Write($" {numBingoCard[i, j].ToString("00")} |");   //formats number

                Console.WriteLine();
            }
            Console.WriteLine("--------------------------");
        }

        //Checks if userInputs matches to NumberBingo Card horizontally
        public static bool CheckHorizontalCard(List<uint> userInput, uint[,] numBingoCard, int vertical, int horizontal)
        {
            bool bingoFlag = true;
            for (int row = 0; row < vertical; ++row) //check x-axis
            {
                bingoFlag = true;
                for (int column = 0; (column < horizontal) && bingoFlag; ++column)
                {
                    uint number = numBingoCard[row, column];
                    bingoFlag = userInput.Contains(number); //tells if userInput contains numbers generated in numBingoCard or not
                }
                if (bingoFlag)
                { //if true Horizontal bingo!
                    Console.WriteLine("+\tBingo! Check Row #" + (row + 1));
                    return true;
                }
            }
            return bingoFlag;
        }

        //Checks if userInputs matches to NumberBingo Card vertically
        public static bool CheckVerticalCard(List<uint> userInput, uint[,] numBingoCard, int vertical, int horizontal)
        {
            bool bingoFlag = true;
            for (int column = 0; column < horizontal; ++column)  //check y-axis
            {
                bingoFlag = true;
                for (int row = 0; (row < vertical) && bingoFlag; ++row)
                {
                    uint number = numBingoCard[row, column];
                    bingoFlag = userInput.Contains(number);  //tells if userInput contains numbers generated in numBingoCard or not
                }
                if (bingoFlag)//if true Vertical bingo!
                {
                    Console.WriteLine("+\tBingo! Check Column #" + (column + 1));
                    return true;
                }
            }
            return bingoFlag;
        }

        //Checks if userInputs matches to NumberBingo Card diagonally (From top left to bottom right)
        public static bool CheckDiagonalFromTopLeftCard(List<uint> userInput, uint[,] numBingoCard, int vertical, int horizontal)
        {
            bool bingoFlag = true;  //set to true until it is flagged as false
            for (int column = 0; (column < horizontal) && bingoFlag; column++)
            {
                for (int row = 0; row < vertical && bingoFlag; ++row)
                {
                    uint number = numBingoCard[row, column];
                    bingoFlag = userInput.Contains(number);
                }
            }
            if (bingoFlag)
            {
                Console.WriteLine("+\tDiagonal Bingo from Top Left to Bottom Right.");
                return true;
            }
            return bingoFlag;
        }

        //Checks if userInputs matches to NumberBingo Card diagonally (From top right to bottom left)
        public static bool CheckDiagonalFromTopRightCard(List<uint> userInput, uint[,] numBingoCard, int vertical, int horizontal)
        {
            bool bingoFlag = true;  //set to true until it is flagged as false
            for (int column = (horizontal - 1); column >= 0 && bingoFlag; column--)
            {
                for (int row = 0; row < vertical && bingoFlag; ++row)
                {
                    uint number = numBingoCard[row, column];
                    bingoFlag = userInput.Contains(number);
                }
            }
            if (bingoFlag)
            {
                Console.WriteLine("+\tDiagonal Bingo from Top Right to Bottom Left.");
                return true;
            }
            return bingoFlag;
        }

        //checks all conditions 
        public static string CheckForNumBingo(List<uint> userInput, uint[,] numBingoCard)
        {
            //x and y axis dimension
            int vertical = numBingoCard.GetUpperBound(0) + 1;
            int horizontal = numBingoCard.GetUpperBound(1) + 1;

            //if one of the conditions returns true or not at all, return the result as string
            return (CheckHorizontalCard(userInput, numBingoCard, vertical, horizontal) ||
                CheckVerticalCard(userInput, numBingoCard, vertical, horizontal) ||
                CheckDiagonalFromTopLeftCard(userInput, numBingoCard, vertical, horizontal) ||
                CheckDiagonalFromTopRightCard(userInput, numBingoCard, vertical, horizontal)
                )
                ?
                "+\tBingo! Bingo! Congrats!" : "+\tNo Bingo! Sorry...";
        }

    }
}
